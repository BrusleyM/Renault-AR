using Common.Interfaces;
using UnityEngine;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]

namespace Helpers
{
    public class PinchZoomHandler
    {
        private float _previousPinchDistance = -1f;
        private Vector3 _previousMidpointWorld;
        private readonly Camera _mainCamera;
        private readonly ITouchProvider _touchProvider;

        public float PreviousPinchDistance => _previousPinchDistance;

        public PinchZoomHandler(Camera camera, ITouchProvider touchProvider)
        {
            _mainCamera = camera;
            _touchProvider = touchProvider;
        }

        public void Handle(GameObject target, float zoomSpeed = 0.001f, float minScale = 0.5f, float maxScale = 10f)
        {
            if (!HasTwoFingerTouch()) return;

            Vector2 touch1, touch2;
            GetTouchPositions(out touch1, out touch2);

            float currentDistance = CalculateTouchDistance(touch1, touch2);
            Vector3 currentMidpointWorld = GetMidpointWorldPosition(touch1, touch2);

            if (IsFirstPinchFrame())
            {
                StoreInitialPinchValues(currentDistance, currentMidpointWorld);
                return;
            }

            HandlePinchZoom(target, currentDistance, currentMidpointWorld, zoomSpeed, minScale, maxScale);
            UpdatePreviousValues(currentDistance, currentMidpointWorld);
        }

        public void Reset()
        {
            _previousPinchDistance = -1f;
        }

        private bool HasTwoFingerTouch()
        {
            return _touchProvider.GetActiveTouches().Count == 2;
        }

        private void GetTouchPositions(out Vector2 touch1, out Vector2 touch2)
        {
            var touches = _touchProvider.GetActiveTouches();
            touch1 = touches[0].position;
            touch2 = touches[1].position;
        }

        private float CalculateTouchDistance(Vector2 touch1, Vector2 touch2)
        {
            return Vector2.Distance(touch1, touch2);
        }

        private bool IsFirstPinchFrame()
        {
            return _previousPinchDistance < 0f;
        }

        internal void StoreInitialPinchValues(float distance, Vector3 midpoint)
        {
            _previousPinchDistance = distance;
            _previousMidpointWorld = midpoint;
        }

        private void HandlePinchZoom(GameObject target, float currentDistance, Vector3 currentMidpointWorld,
                                   float zoomSpeed, float minScale, float maxScale)
        {
            float pinchDelta = currentDistance - _previousPinchDistance;
            float scaleChange = CalculateScaleChange(pinchDelta, zoomSpeed);

            Vector3 newScale = CalculateNewScale(target.transform.localScale, scaleChange, minScale, maxScale);
            target.transform.localScale = newScale;

            UpdateObjectPosition(target, currentMidpointWorld);
        }

        internal float CalculateScaleChange(float pinchDelta, float zoomSpeed)
        {
            return 1 + pinchDelta * zoomSpeed;
        }

        internal Vector3 CalculateNewScale(Vector3 currentScale, float scaleChange, float minScale, float maxScale)
        {
            Vector3 newScale = currentScale * scaleChange;
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);
            return newScale;
        }

        internal void UpdateObjectPosition(GameObject target, Vector3 currentMidpointWorld)
        {
            Vector3 worldDelta = currentMidpointWorld - _previousMidpointWorld;
            target.transform.position += worldDelta;
        }

        private void UpdatePreviousValues(float currentDistance, Vector3 currentMidpointWorld)
        {
            _previousPinchDistance = currentDistance;
            _previousMidpointWorld = currentMidpointWorld;
        }

        internal Vector3 GetMidpointWorldPosition(Vector2 screenPointA, Vector2 screenPointB)
        {
            Vector2 screenMid = (screenPointA + screenPointB) * 0.5f;
            Ray ray = _mainCamera.ScreenPointToRay(screenMid);

            if (Physics.Raycast(ray, out RaycastHit hit))
                return hit.point;

            return ray.GetPoint(2f);
        }
    }
}