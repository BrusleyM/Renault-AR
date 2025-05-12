using System;
using Common.Objects;
using Helpers;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using EnhanceTouch = UnityEngine.InputSystem.EnhancedTouch;

public class ARInputHandler : IARInputHandler
{
    public event Action<CommonTouch> OnTouchDown;
    public event Action<CommonTouch> OnTouchMove;
    public event Action OnTouchUp;

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

        EnhanceTouch.Touch.onFingerDown += finger =>
            OnTouchDown?.Invoke(new CommonTouch(finger.screenPosition));

        EnhanceTouch.Touch.onFingerMove += finger =>
            OnTouchMove?.Invoke(new CommonTouch(finger.screenPosition));

        EnhanceTouch.Touch.onFingerUp += _ => OnTouchUp?.Invoke();
    }
}