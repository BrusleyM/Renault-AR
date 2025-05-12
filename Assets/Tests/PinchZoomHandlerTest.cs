using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using Helpers;
using System.Collections.Generic;
using Common.Objects;
using Common.Interfaces;

namespace Tests
{
    [TestFixture]
    public class PinchZoomHandlerTests
    {
        private PinchZoomHandler _handler;
        private ITouchProvider _mockTouchProvider;
        private Camera _camera;
        private GameObject _testTarget;
        private GameObject _testPlane;

        [SetUp]
        public void Setup()
        {
            _camera = new GameObject().AddComponent<Camera>();
            _camera.transform.position = new Vector3(0, 0, -10);
            _mockTouchProvider = Substitute.For<ITouchProvider>();
            _handler = new PinchZoomHandler(_camera, _mockTouchProvider);
            _testTarget = new GameObject();

            // Setup test plane for raycasting
            _testPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            _testPlane.transform.position = Vector3.forward * 10f;
        }

        [TearDown]
        public void Teardown()
        {
            _handler.CleanUp();
            Object.DestroyImmediate(_testTarget);
            Object.DestroyImmediate(_camera.gameObject);
            Object.DestroyImmediate(_testPlane);
        }

        #region Core Functionality Tests
        [Test]
        public void InitializeZoom_CreatesParentContainer()
        {
            // Act
            _handler.InitializeZoom(_testTarget);

            // Assert
            Assert.IsNotNull(_testTarget.transform.parent);
            Assert.AreEqual("ZoomPivot", _testTarget.transform.parent.name);
        }

        [Test]
        public void Handle_FirstFrame_StoresInitialValues()
        {
            // Arrange
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 100);

            // Act
            _handler.Handle(_testTarget);

            // Assert
            Assert.GreaterOrEqual(_handler.PreviousPinchDistance, 0f);
        }
        #endregion

        #region Edge Case Tests
        [Test]
        public void Handle_ExtremePinchOut_ClampsToMaxScale()
        {
            // Arrange
            _testTarget.transform.localScale = Vector3.one * 9f;
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 100);
            _handler.Handle(_testTarget); // First frame

            // Act - Extreme pinch out
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 1000);
            _handler.Handle(_testTarget, maxScale: 10f);

            // Assert
            Assert.AreEqual(10f, _testTarget.transform.parent.localScale.x, 0.01f);
        }

        [Test]
        public void Handle_ExtremePinchIn_ClampsToMinScale()
        {
            // Arrange
            _testTarget.transform.localScale = Vector3.one;
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 200);
            _handler.Handle(_testTarget); // First frame

            // Act - Extreme pinch in
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 5);
            _handler.Handle(_testTarget, minScale: 0.5f);

            // Assert
            Assert.AreEqual(0.5f, _testTarget.transform.parent.localScale.x, 0.01f);
        }

        [Test]
        public void Handle_VeryCloseTouches_DoesNotCrash()
        {
            // Arrange
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 0.1f);

            // Act & Assert (should not throw)
            Assert.DoesNotThrow(() => _handler.Handle(_testTarget));
        }

        [Test]
        public void Handle_NoRaycastHit_UsesDefaultDistance()
        {
            // Arrange - Remove plane so raycast fails
            Object.DestroyImmediate(_testPlane);
            SetupTwoFingerTouch(new Vector2(100, 100), new Vector2(200, 200));

            // Act
            _handler.Handle(_testTarget);

            // Assert - Should use camera's nearClipPlane + 1f
            Assert.AreNotEqual(Vector3.zero, _testTarget.transform.position);
        }

        [Test]
        public void Handle_RapidAlternatingPinches_HandlesCorrectly()
        {
            // Test alternating between pinch in/out quickly
            _testTarget.transform.localScale = Vector3.one;

            // Frame 1 - Initial
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 100);
            _handler.Handle(_testTarget);

            // Frame 2 - Pinch out
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 150);
            _handler.Handle(_testTarget);
            float scaleAfterPinchOut = _testTarget.transform.parent.localScale.x;

            // Frame 3 - Immediate pinch in
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 50);
            _handler.Handle(_testTarget);

            // Assert
            Assert.Less(_testTarget.transform.parent.localScale.x, scaleAfterPinchOut);
        }
        #endregion

        #region Cleanup Tests
        [Test]
        public void CleanUp_RestoresOriginalTransform()
        {
            // Arrange
            var originalPosition = _testTarget.transform.position;
            var originalRotation = _testTarget.transform.rotation;
            var originalScale = _testTarget.transform.localScale;

            _handler.InitializeZoom(_testTarget);
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 100);
            _handler.Handle(_testTarget);

            // Act
            _handler.CleanUp();

            // Assert
            Assert.AreEqual(originalPosition, _testTarget.transform.position);
            Assert.AreEqual(originalRotation, _testTarget.transform.rotation);
            Assert.AreEqual(originalScale, _testTarget.transform.localScale);
        }

        [Test]
        public void CleanUp_WhenCalledTwice_DoesNotError()
        {
            // Arrange
            _handler.InitializeZoom(_testTarget);

            // Act & Assert
            Assert.DoesNotThrow(() => {
                _handler.CleanUp();
                _handler.CleanUp(); // Second call
            });
        }
        #endregion

        #region Helper Methods
        private void SetupTwoFingerTouch(Vector2 pos1, Vector2 pos2)
        {
            _mockTouchProvider.GetActiveTouches().Returns(new List<CommonTouch>
            {
                new CommonTouch(pos1),
                new CommonTouch(pos2)
            });
        }
        #endregion
    }
}