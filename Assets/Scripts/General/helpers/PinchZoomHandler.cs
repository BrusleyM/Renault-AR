using Common.Interfaces;
using UnityEngine;

namespace Helpers
{
    public class PinchZoomHandler
    {
        private float _previousPinchDistance = -1f;
        private Vector3 _previousMidpointWorld;
        private readonly Camera _mainCamera;
        private readonly ITouchProvider _touchProvider;

        public PinchZoomHandler(Camera camera, ITouchProvider touchProvider)
        {
            _mainCamera = camera;
            _touchProvider = touchProvider;
        }

        public void Handle(GameObject target)
        {
            var activeTouches = _touchProvider.GetActiveTouches();
            if (activeTouches.Count != 2) return;

            Vector2 touch1 = activeTouches[0].position;
            Vector2 touch2 = activeTouches[1].position;

            float currentDistance = Vector2.Distance(touch1, touch2);
            Vector3 currentMidpointWorld = GetMidpointWorldPosition(touch1, touch2);

            if (_previousPinchDistance < 0f)
            {
                _previousPinchDistance = currentDistance;
                _previousMidpointWorld = currentMidpointWorld;
                return;
            }

            float pinchDelta = currentDistance - _previousPinchDistance;
            float zoomSpeed = 0.001f;
            float scaleChange = 1 + pinchDelta * zoomSpeed;

            scaleChange = Mathf.Clamp(scaleChange, 0.98f, 1.02f);
            Vector3 newScale = target.transform.localScale * scaleChange;

            float minScale = 0.5f;
            float maxScale = 10f;
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);

            target.transform.localScale = newScale;

            Vector3 worldDelta = currentMidpointWorld - _previousMidpointWorld;
            target.transform.position += worldDelta;

            _previousPinchDistance = currentDistance;
            _previousMidpointWorld = currentMidpointWorld;
        }

        public void Reset()
        {
            _previousPinchDistance = -1f;
        }

        private Vector3 GetMidpointWorldPosition(Vector2 screenPointA, Vector2 screenPointB)
        {
            Vector2 screenMid = (screenPointA + screenPointB) * 0.5f;
            Ray ray = _mainCamera.ScreenPointToRay(screenMid);

            if (Physics.Raycast(ray, out RaycastHit hit))
                return hit.point;

            return ray.GetPoint(2f);
        }
    }
}
