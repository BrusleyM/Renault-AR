using Helpers;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.XR.ARFoundation;
using Services;
using Common.interfaces;

namespace ARscene
{
    [RequireComponent(typeof(ARPlaneManager), typeof(ARRaycastManager))]
    public class InteractionsManager : MonoBehaviour
    {
        [SerializeField] private ARSession _session;
        [SerializeField] private GameObject _featuresUI;
        [SerializeField] private ARAnchorManager _anchorManager;

        private ARPlaneManager _planeManager;
        private ARRaycastManager _rayManager;
        private IARCarPlacer _carPlacer;
        private IARInputHandler _inputHandler;

        void Awake()
        {
            _planeManager = GetComponent<ARPlaneManager>();
            _rayManager = GetComponent<ARRaycastManager>();

            _carPlacer = new ARCarPlacer(_planeManager, _rayManager, _anchorManager);
            _inputHandler = new ARInputHandler(new PinchZoomHandler(Camera.main, new RuntimeTouchProvider()));
        }

        void Start()
        {
            _session.Reset();
            _featuresUI.SetActive(false);
        }

        void OnEnable()
        {
            _inputHandler.OnFingerDown += HandleFingerDown;
            _inputHandler.OnFingerMove += HandleFingerMove;
            _inputHandler.OnFingerUp += HandleFingerUp;
        }

        void OnDisable()
        {
            _inputHandler.OnFingerDown -= HandleFingerDown;
            _inputHandler.OnFingerMove -= HandleFingerMove;
            _inputHandler.OnFingerUp -= HandleFingerUp;
        }

        private void HandleFingerDown(Finger finger)
        {
            if (_carPlacer.HasPlacedCar) return;

            _carPlacer.TryPlaceCar(finger, GameManager.Instance.SelectedCar, (car) =>
            {
                GameManager.Instance.SetInstantiatedCar(car);
                _featuresUI.SetActive(true);
            });
        }

        private void HandleFingerMove(Finger finger)
        {
            if (!_carPlacer.HasPlacedCar) return;
            _inputHandler.HandlePinchZoom(GameManager.Instance.InstantiatedCar);
        }

        private void HandleFingerUp(Finger finger)
        {
            _inputHandler.ResetPinchZoom();
        }
    }
}