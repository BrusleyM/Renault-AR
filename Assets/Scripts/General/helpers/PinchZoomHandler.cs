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
        private GameObject _zoomParent;
        private Vector3 _originalChildPosition;

        public float PreviousPinchDistance => _previousPinchDistance;

        public PinchZoomHandler(Camera camera, ITouchProvider touchProvider)
        {
            _mainCamera = camera;
            _touchProvider = touchProvider;
        }

        public void InitializeZoom(GameObject target)
        {
            if (_zoomParent == null || target.transform.parent != _zoomParent)
            {
                // Create new parent container
                _zoomParent = new GameObject("ZoomPivot");
                _zoomParent.transform.position = target.transform.position;
                _zoomParent.transform.rotation = target.transform.rotation;

                // Store original local position before reparenting
                _originalChildPosition = target.transform.localPosition;

                // Reparent while maintaining world position
                target.transform.SetParent(_zoomParent.transform, true);
            }
        }

        public void Handle(GameObject target, float zoomSpeed = 0.001f, float minScale = 0.5f, float maxScale = 10f)
        {
            if (!HasTwoFingerTouch()) return;

            InitializeZoom(target);

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

        private void HandlePinchZoom(GameObject target, float currentDistance, Vector3 currentMidpointWorld,
                                   float zoomSpeed, float minScale, float maxScale)
        {
            float pinchDelta = currentDistance - _previousPinchDistance;
            float scaleFactor = CalculateScaleChange(pinchDelta, zoomSpeed);

            // Get current and new scale
            Vector3 currentParentScale = _zoomParent.transform.localScale;
            Vector3 newScale = currentParentScale * scaleFactor;

            // Apply clamping
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);

            // Convert focus point to parent's local space
            Vector3 focusPointLocal = _zoomParent.transform.InverseTransformPoint(currentMidpointWorld);

            // Calculate new local position to maintain focus point
            float scaleRatio = newScale.x / currentParentScale.x;
            Vector3 newLocalPosition = focusPointLocal - (focusPointLocal - target.transform.localPosition) * scaleRatio;

            // Apply transformations
            _zoomParent.transform.localScale = newScale;
            target.transform.localPosition = newLocalPosition;
        }

        public void Reset()
        {
            _previousPinchDistance = -1f;
        }

        public void CleanUp()
        {
            if (_zoomParent != null)
            {
                // Restore original hierarchy if needed
                if (_zoomParent.transform.childCount > 0)
                {
                    Transform child = _zoomParent.transform.GetChild(0);
                    child.SetParent(null, true);
                    child.position = _zoomParent.transform.position;
                    child.rotation = _zoomParent.transform.rotation;
                    child.localScale = _zoomParent.transform.localScale;
                }
                GameObject.Destroy(_zoomParent);
                _zoomParent = null;
            }
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

        internal float CalculateScaleChange(float pinchDelta, float zoomSpeed)
        {
            return 1 + pinchDelta * zoomSpeed;
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

            return ray.GetPoint(_mainCamera.nearClipPlane + 1f);  // More reliable distance
        }
    }
}