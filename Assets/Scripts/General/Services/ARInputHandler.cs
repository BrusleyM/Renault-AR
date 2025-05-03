using System;
using System.Collections;
using System.Collections.Generic;
using Common.interfaces;
using Helpers;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhanceTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace Services
{
    public class ARInputHandler : IARInputHandler
    {
        public event Action<Finger> OnFingerDown;
        public event Action<Finger> OnFingerMove;
        public event Action<Finger> OnFingerUp;

        private readonly PinchZoomHandler _pinchZoomHandler;

        public ARInputHandler(PinchZoomHandler pinchZoomHandler)
        {
            _pinchZoomHandler = pinchZoomHandler;
            EnableTouchEvents();
        }

        public void HandlePinchZoom(GameObject target) => _pinchZoomHandler.Handle(target);
        public void ResetPinchZoom() => _pinchZoomHandler.Reset();

        private void EnableTouchEvents()
        {
            TouchSimulation.Enable();
            EnhancedTouchSupport.Enable();
            EnhanceTouch.Touch.onFingerDown += finger => OnFingerDown?.Invoke(finger);
            EnhanceTouch.Touch.onFingerMove += finger => OnFingerMove?.Invoke(finger);
            EnhanceTouch.Touch.onFingerUp += finger => OnFingerUp?.Invoke(finger);
        }
    }
}
