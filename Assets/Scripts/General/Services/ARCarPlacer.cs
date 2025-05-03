using System;
using System.Collections.Generic;
using System.Linq;
using Common.interfaces;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Services
{
    public class ARCarPlacer : IARCarPlacer
    {
        private readonly ARPlaneManager _planeManager;
        private readonly ARRaycastManager _rayManager;
        private readonly ARAnchorManager _anchorManager;
        private GameObject _placedCar;

        public bool HasPlacedCar => _placedCar != null;

        public ARCarPlacer(ARPlaneManager planeManager, ARRaycastManager rayManager, ARAnchorManager anchorManager)
        {
            _planeManager = planeManager;
            _rayManager = rayManager;
            _anchorManager = anchorManager;
        }

        public void TryPlaceCar(Finger finger, GameObject carPrefab, Action<GameObject> onPlaced)
        {
            if (finger.index != 0 || HasPlacedCar) return;

            var hits = new List<ARRaycastHit>();
            if (_rayManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                var hit = hits.FirstOrDefault(h =>
                    _planeManager.GetPlane(h.trackableId).alignment == PlaneAlignment.HorizontalUp);

                if (hit.Equals(default(ARRaycastHit))) return;

                Pose pose = hit.pose;
                _placedCar = UnityEngine.Object.Instantiate(carPrefab, pose.position, pose.rotation);

                // Face camera
                Vector3 camPos = Camera.main.transform.position;
                Vector3 direction = camPos - _placedCar.transform.position;
                direction.y = 0f;
                _placedCar.transform.rotation = Quaternion.LookRotation(direction);

                // Create anchor
                var plane = _planeManager.GetPlane(hit.trackableId);
                var anchor = _anchorManager.AttachAnchor(plane, new Pose(_placedCar.transform.position, _placedCar.transform.rotation));
                if (anchor != null) _placedCar.transform.SetParent(anchor.transform);

                DisableOtherPlanes(plane);
                onPlaced?.Invoke(_placedCar);
            }
        }

        private void DisableOtherPlanes(ARPlane keepPlane)
        {
            foreach (var plane in _planeManager.trackables)
                plane.gameObject.SetActive(plane == keepPlane);
        }
    }
}
