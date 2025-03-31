using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WelcomeManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown carDropdown;
    [SerializeField]
    private GameObject[] carPrefabs;
    [SerializeField]
    TMP_InputField _name;
    [SerializeField]
    TMP_InputField _surname;
    [SerializeField]
    TMP_InputField _id;
    [SerializeField]
    Button _launchButton;
    Person _userInfo=new Person();
    private void OnEnable()
    {
        _launchButton.onClick.AddListener(OnLaunchPressed);
    }

    public void OnLaunchPressed()
    {
        int selectedIndex = carDropdown.value;
        SetUser();

        GameManager.Instance.SetUserInfo(_userInfo);
        GameManager.Instance.SetSelectedCar(carPrefabs[selectedIndex], carDropdown.options[selectedIndex].text);
        GameManager.Instance.LoadScene("AR");
    }
    void SetUser()
    {
        _userInfo.Name = _name.text;
        _userInfo.Surname = _surname.text;
        _userInfo.ID = _id.text;
    }
}