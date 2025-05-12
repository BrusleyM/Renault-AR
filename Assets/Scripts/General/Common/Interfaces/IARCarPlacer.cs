using System;
using Common.Objects;
using UnityEngine;

namespace Common.Interfaces
{
    public interface IARCarPlacer
    {
        bool HasPlacedCar { get; }
        void TryPlaceCar(CommonTouch finger, GameObject carPrefab, Action<GameObject> onPlaced);
    }
}
