using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Common.interfaces
{
    public interface IARInputHandler
    {
        event Action<Finger> OnFingerDown;
        event Action<Finger> OnFingerMove;
        event Action<Finger> OnFingerUp;

        void HandlePinchZoom(GameObject target);
        void ResetPinchZoom();
    }
}
