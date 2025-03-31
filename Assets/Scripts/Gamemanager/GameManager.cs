using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    Person _userInfo=new Person();
    public Person UserInfo => _userInfo;
    private GameObject _selectedCar;
    public GameObject SelectedCar => _selectedCar;
    private string _carName;
    public string CarName => _carName;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSelectedCar(GameObject carPrefab,string name)
    {
        _selectedCar = carPrefab;
        _carName = name;
    }
    public void SetUserInfo(Person person)
    {
        _userInfo = person;
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}