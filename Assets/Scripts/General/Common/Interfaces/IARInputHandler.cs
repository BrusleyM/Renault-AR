using System;
using Common.Objects;
using UnityEngine;

public interface IARInputHandler
{
    event Action<CommonTouch> OnTouchDown;
    event Action<CommonTouch> OnTouchMove;
    event Action OnTouchUp; 

    void HandlePinchZoom(GameObject target);
    void ResetPinchZoom();
}