using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Common.interfaces
{
    public interface IARCarPlacer
    {
        bool HasPlacedCar { get; }
        void TryPlaceCar(Finger finger, GameObject carPrefab, Action<GameObject> onPlaced);
    }
}
