using Common.Objects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ARscene
{
    public class CarFeatures : MonoBehaviour
    {
        [SerializeField]
        List<Button> _btnColours;
        [SerializeField]
        Button _back;
        [SerializeField]
        List<Material> _colours;
        [SerializeField]
        TMP_Text _carName;
        CarDetails _carDetails;
        private void Start()
        {
            _carDetails= GameManager.Instance.SelectedCar.GetComponent<CarDetails>();
            _carName.text = GameManager.Instance.CarName;
            foreach(var btn in _btnColours)
            {
                btn.onClick.AddListener(() =>ChangeColour(_btnColours.FindIndex(a=>a==btn)));
            }
            _back.onClick.AddListener(() =>
            {
                GameManager.Instance.LoadScene("Menu");
            });
        }
        void ChangeColour(int colorIndex)
        {

            foreach(var part in _carDetails.BodyParts)
            {
                part.GetComponent<Renderer>().material = _colours[colorIndex];
            }
        }

    }
}
