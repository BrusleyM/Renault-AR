using Common.interfaces;
using Common.Objects;
using Services;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour, IGameManager
    {
        private static IGameManager _instance;
        public static IGameManager Instance => _instance;

        private Person _userInfo = new();
        private GameObject _selectedCar;
        private GameObject _instantiatedCar;
        private string _carName;

        public Person UserInfo => _userInfo;
        public GameObject SelectedCar => _selectedCar;
        public GameObject InstantiatedCar => _instantiatedCar;
        public string CarName => _carName;

        private ISceneLoader _sceneLoader;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                _sceneLoader = new UnitySceneLoader();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetSelectedCar(GameObject carPrefab, string name)
        {
            _selectedCar = carPrefab;
            _carName = name;
        }
        public void SetInstantiatedCar(GameObject carPrefab)
        {
            _instantiatedCar = carPrefab;
        }
        public void SetUserInfo(Person person)
        {
            _userInfo = person;
        }

        public void SetSceneLoader(ISceneLoader loader)
        {
            _sceneLoader = loader;
        }

        public void LoadScene(string sceneName)
        {
            _sceneLoader?.Load(sceneName);
        }
    }
}