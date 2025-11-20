using Common.Objects;
using UnityEngine;

namespace Common.Interfaces
{
    public interface IGameManager
    {
        SelectedCar SelectedCar { get; }
        GameObject InstantiatedCar { get; }
        Person UserInfo { get; }

        void SetSelectedCar(SelectedCar car);
        void SetInstantiatedCar(GameObject car);
        void SetUserInfo(Person person);
        void LoadScene(string sceneName);
    }
}
