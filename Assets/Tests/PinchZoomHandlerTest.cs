using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using Common.Interfaces;
using Helpers;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.TestTools;
using Common.Objects;

namespace Tests
{
    [TestFixture]
    public class PinchZoomHandlerTests
    {
        private PinchZoomHandler _handler;
        private ITouchProvider _touchProvider;
        private Camera _camera;
        private GameObject _testTarget;

        [SetUp]
        public void Setup()
        {
            _camera = new GameObject().AddComponent<Camera>();
            _touchProvider = Substitute.For<ITouchProvider>();
            _handler = new PinchZoomHandler(_camera, _touchProvider);
            _testTarget = new GameObject();
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(_camera.gameObject);
            Object.DestroyImmediate(_testTarget);
        }

        #region Core Functionality Tests
        [Test]
        public void Handle_WithTwoFingerTouch_ProcessesInput()
        {
            // Arrange - First frame (stores initial values)
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 100);
            _handler.Handle(_testTarget);

            // Act - Second frame (should process scaling)
            SetupTwoFingerTouch(Vector2.zero, Vector2.right * 150); // 50px increase
            _handler.Handle(_testTarget);

            // Assert
            Assert.AreNotEqual(Vector3.one, _testTarget.transform.localScale);
            Assert.Greater(_testTarget.transform.localScale.x, 1f); // Should scale up
        }

        [Test]
        public void Handle_WithSingleTouch_DoesNothing()
        {
            // Arrange
            _touchProvider.GetActiveTouches().Returns(new List<CommonTouch> { new CommonTouch(Vector2.zero) });
            var originalScale = _testTarget.transform.localScale;

            // Act
            _handler.Handle(_testTarget);

            // Assert
            Assert.AreEqual(originalScale, _testTarget.transform.localScale);
        }
        #endregion

        #region Scale Calculation Tests
        [Test]
        public void CalculateScaleChange_PositiveDelta_ReturnsIncreasedScale()
        {
            // Act
            float result = _handler.CalculateScaleChange(100f, 0.001f);

            // Assert
            Assert.AreEqual(1.1f, result);
        }

        [Test]
        public void CalculateScaleChange_NegativeDelta_ReturnsDecreasedScale()
        {
            // Act
            float result = _handler.CalculateScaleChange(-50f, 0.001f);

            // Assert
            Assert.AreEqual(0.95f, result);
        }

        [Test]
        public void CalculateNewScale_AboveMax_ClampsToMax()
        {
            // Act
            Vector3 result = _handler.CalculateNewScale(
                Vector3.one * 9f, 1.2f, 0.5f, 10f);

            // Assert
            Assert.AreEqual(10f, result.x);
        }

        [Test]
        public void CalculateNewScale_BelowMin_ClampsToMin()
        {
            // Act
            Vector3 result = _handler.CalculateNewScale(
                Vector3.one * 0.4f, 0.9f, 0.5f, 10f);

            // Assert
            Assert.AreEqual(0.5f, result.x);
        }

        [Test]
        public void CalculateNewScale_UniformScaling_MaintainsProportions()
        {
            // Arrange
            Vector3 unevenScale = new Vector3(1f, 1.5f, 2f);

            // Act
            Vector3 result = _handler.CalculateNewScale(
                unevenScale, 1.1f, 0.5f, 10f);

            // Assert
            Assert.AreEqual(result.x * 1.5f, result.y);
            Assert.AreEqual(result.x * 2f, result.z);
        }
        #endregion

        #region Position Update Tests
        [Test]
        public void UpdateObjectPosition_WithValidDelta_MovesObject()
        {
            // Arrange
            GameObject target = new GameObject();
            Vector3 originalPos = target.transform.position;

            // Act
            _handler.UpdateObjectPosition(target, Vector3.forward * 2f);

            // Assert
            Assert.AreNotEqual(originalPos, target.transform.position);

            // Cleanup
            Object.DestroyImmediate(target);
        }

        [UnityTest]
        public IEnumerator GetMidpointWorldPosition_WithRaycastHit_ReturnsHitPoint()
        {
            // Arrange
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.transform.position = Vector3.forward * 5f;
            yield return null; // Wait for physics setup

            // Act
            Vector3 result = _handler.GetMidpointWorldPosition(
                new Vector2(Screen.width / 2, Screen.height / 2),
                new Vector2(Screen.width / 2 + 100, Screen.height / 2));

            // Assert
            Assert.AreNotEqual(Vector3.zero, result);

            // Cleanup
            Object.DestroyImmediate(plane);
        }
        #endregion

        #region State Management Tests
        [Test]
        public void StoreInitialPinchValues_SetsStateCorrectly()
        {
            // Act
            _handler.StoreInitialPinchValues(100f, Vector3.forward);

            // Assert
            Assert.AreEqual(100f, _handler.PreviousPinchDistance);
        }

        [Test]
        public void Reset_ClearsPreviousValues()
        {
            // Arrange
            _handler.StoreInitialPinchValues(100f, Vector3.forward);

            // Act
            _handler.Reset();

            // Assert
            Assert.AreEqual(-1f, _handler.PreviousPinchDistance);
        }
        #endregion

        #region Helper Methods
        private void SetupTwoFingerTouch(Vector2 pos1, Vector2 pos2)
        {
            _touchProvider.GetActiveTouches().Returns(new List<CommonTouch>
    {
        new CommonTouch(pos1),
        new CommonTouch(pos2)
    });
        }
        #endregion
    }
}