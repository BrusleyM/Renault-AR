using Helpers;
using Managers;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Services;
using Common.Interfaces;
using Common.Objects;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace ARscene
{
    [RequireComponent(typeof(ARPlaneManager), typeof(ARRaycastManager))]
    public class InteractionsManager : MonoBehaviour
    {
        [SerializeField] internal ARSession _session;
        [SerializeField] internal GameObject _featuresUI;
        [SerializeField] internal ARAnchorManager _anchorManager;

        private ARPlaneManager _planeManager;
        private ARRaycastManager _rayManager;
        private ARCarPlacer _carPlacer;
        private ARInputHandler _inputHandler;

        private Action<CommonTouch> _onTouchDown;
        private Action<CommonTouch> _onTouchMove;
        private Action _onTouchUp;

        void Awake()
        {
            _planeManager = GetComponent<ARPlaneManager>();
            _rayManager = GetComponent<ARRaycastManager>();

            _carPlacer = new ARCarPlacer(_planeManager, _rayManager, _anchorManager);

            var pinchHandler = new PinchZoomHandler(Camera.main, new RuntimeTouchProvider());
            _inputHandler = new ARInputHandler(pinchHandler);

            _onTouchDown = HandleTouchDown;
            _onTouchMove = HandleTouchMove;
            _onTouchUp = HandleTouchUp;
        }

        internal void Start()
        {
            _session.Reset();
            _featuresUI.SetActive(false);
        }

        internal void OnEnable()
        {
            _inputHandler.OnTouchDown += _onTouchDown;
            _inputHandler.OnTouchMove += _onTouchMove;
            _inputHandler.OnTouchUp += _onTouchUp;
        }

        internal void OnDisable()
        {
            _inputHandler.OnTouchDown -= _onTouchDown;
            _inputHandler.OnTouchMove -= _onTouchMove;
            _inputHandler.OnTouchUp -= _onTouchUp;
        }

        internal void HandleTouchDown(CommonTouch touch)
        {
            if (_carPlacer.HasPlacedCar) return;

            _carPlacer.TryPlaceCar(touch, GameManager.Instance.SelectedCar.Car, car =>
            {
                GameManager.Instance.SetInstantiatedCar(car);
                _featuresUI.SetActive(true);
            });
        }

        internal void HandleTouchMove(CommonTouch touch)
        {
            if (!_carPlacer.HasPlacedCar) return;
            _inputHandler.HandlePinchZoom(GameManager.Instance.InstantiatedCar);
        }

        internal void HandleTouchUp()
        {
            _inputHandler.ResetPinchZoom();
        }
    }
}