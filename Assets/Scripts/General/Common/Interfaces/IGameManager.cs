using System.Collections;
using System.Collections.Generic;
using Common.Objects;
using UnityEngine;

namespace Common.interfaces
{
    public interface IGameManager
    {
        GameObject SelectedCar { get; }
        GameObject InstantiatedCar { get; }
        string CarName { get; }
        Person UserInfo { get; }

        void SetSelectedCar(GameObject carPrefab, string name);
        void SetInstantiatedCar(GameObject car);
        void SetUserInfo(Person person);
        void LoadScene(string sceneName);
    }
}
