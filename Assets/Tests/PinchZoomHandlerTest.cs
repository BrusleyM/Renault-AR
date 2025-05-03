using System.Collections;
using Helpers;
using Common.Objects;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PinchZoomHandlerTests
    {
        private GameObject _target;
        private Camera _camera;
        private PinchZoomHandler _handler;
        private MockTouchProvider _mockTouchProvider;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _target.transform.position = Vector3.zero;
            _target.transform.localScale = Vector3.one;

            var cameraGO = new GameObject("TestCamera");
            _camera = cameraGO.AddComponent<Camera>();
            _camera.transform.position = new Vector3(0, 0, -10);

            _mockTouchProvider = new MockTouchProvider();
            _handler = new PinchZoomHandler(_camera, _mockTouchProvider);

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(_target);
            Object.Destroy(_camera.gameObject);
            yield return null;
        }

        [UnityTest]
        public IEnumerator PinchZoomHandler_ZoomsTargetObject()
        {
            Vector2 initialTouch1 = new Vector2(Screen.width * 0.4f, Screen.height * 0.5f);
            Vector2 initialTouch2 = new Vector2(Screen.width * 0.6f, Screen.height * 0.5f);
            _mockTouchProvider.SetTouches(
                new CommonTouch(initialTouch1),
                new CommonTouch(initialTouch2)
            );

            yield return null;

            _handler.Handle(_target);
            Vector3 initialScale = _target.transform.localScale;

            // Simulate pinch out (fingers move apart)
            Vector2 movedTouch1 = new Vector2(Screen.width * 0.3f, Screen.height * 0.5f);
            Vector2 movedTouch2 = new Vector2(Screen.width * 0.7f, Screen.height * 0.5f);
            _mockTouchProvider.SetTouches(
                new CommonTouch(movedTouch1),
                new CommonTouch(movedTouch2)
            );

            yield return null;

            _handler.Handle(_target);
            Vector3 newScale = _target.transform.localScale;

            Assert.Greater(newScale.x, initialScale.x, "Target should scale up during pinch out.");
        }
    }
}