using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhanceTouch = UnityEngine.InputSystem.EnhancedTouch;

public class InteractionsManager : MonoBehaviour
{

    GameObject _carPrefab;
    GameObject _car;
    ARPlaneManager _planeManager;
    ARRaycastManager _rayManager;
    List<ARRaycastHit> _hits = new List<ARRaycastHit>();
    [SerializeField] ARSession _session;
    [SerializeField] GameObject _featuresUI;

    float _initialPinchDistance;
    Vector3 _initialScale;
    void Awake()
    {
        _planeManager = GetComponent<ARPlaneManager>();
        _rayManager = GetComponent<ARRaycastManager>();
        _carPrefab = GameManager.Instance.SelectedCar;
    }
    private void Start()
    {
        _session.Reset();
        _featuresUI.SetActive(false);
    }
    private void OnEnable()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        EnhanceTouch.Touch.onFingerDown += PlaceCar;
        EnhanceTouch.Touch.onFingerMove += ManipulateScale;
    }

    private void OnDisable()
    {
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
        EnhanceTouch.Touch.onFingerDown -= PlaceCar;
        EnhanceTouch.Touch.onFingerMove -= ManipulateScale;

    }
    private void PlaceCar(Finger finger)
    {
        if (finger.index != 0 && _car != null)
            return;
        if (_rayManager.Raycast(finger.currentTouch.screenPosition, _hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in _hits)
            {
                Pose pose = hit.pose;

                ARPlane plane = _planeManager.GetPlane(hit.trackableId);

                foreach (var existingPlane in _planeManager.trackables)
                {
                    existingPlane.gameObject.SetActive(existingPlane == plane);
                }
                _car = Instantiate(_carPrefab, pose.position, pose.rotation);
                if (plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    Vector3 pos = _car.transform.position;
                    Vector3 camerapos = Camera.main.transform.position;
                    Vector3 dir = camerapos - pos;
                    Vector3 targetRotEuler = Quaternion.LookRotation(dir).eulerAngles;
                    var scaled = Vector3.Scale(targetRotEuler, _car.transform.up.normalized);
                    Quaternion targetRot = Quaternion.Euler(scaled);
                    _car.transform.rotation = _car.transform.rotation * targetRot;
                    StopPlaneDetection();
                }

            }
        }
        _featuresUI.SetActive(true);

        EnhanceTouch.Touch.onFingerDown -= PlaceCar;
    }
    public void StopPlaneDetection()
    {
        if (_planeManager != null)
        {
            _planeManager.enabled = false; // Stop detecting new planes

            foreach (var plane in _planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
    }
    private void ManipulateScale(Finger finger)
    {
        var activeTouches = EnhanceTouch.Touch.activeTouches;
        if (_car == null || activeTouches.Count != 2) return; // Ensure two fingers are touching

        Vector2 touch1 = activeTouches[0].screenPosition;
        Vector2 touch2 = activeTouches[1].screenPosition;

        float currentDistance = Vector2.Distance(touch1, touch2);

        if (_initialPinchDistance == 0)
        {
            _initialPinchDistance = currentDistance;
            _initialScale = _car.transform.localScale;
        }
        else
        {
            float scaleFactor = currentDistance / _initialPinchDistance;
            _car.transform.localScale = _initialScale * scaleFactor;

            // Optional: Clamp scale to prevent extreme values
            float minScale = .5f;
            float maxScale = 10.0f;
            _car.transform.localScale = new Vector3(
                Mathf.Clamp(_car.transform.localScale.x, minScale, maxScale),
                Mathf.Clamp(_car.transform.localScale.y, minScale, maxScale),
                Mathf.Clamp(_car.transform.localScale.z, minScale, maxScale)
            );
        }
    }
}
