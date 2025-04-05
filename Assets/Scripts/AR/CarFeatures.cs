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
        Button _back;
        [SerializeField]
        List<Material> _colours;
        [SerializeField]
        TMP_Text _carName;
        CarDetails _carDetails;
        private void Awake()
        {
            _carDetails = GameManager.Instance.SelectedCar.GetComponent<CarDetails>();
            _carName.text = GameManager.Instance.CarName;
        }
        private void Start()
        {
            _back.onClick.AddListener(() =>
            {
                Debug.Log("loaded");
                GameManager.Instance.LoadScene("Menu");
            });
        }
        public void Testing(int index)
        {
            foreach (var part in _carDetails.BodyParts)
            {
                part.GetComponent<Renderer>().material = _colours[index];
            }
        }

    }
}
