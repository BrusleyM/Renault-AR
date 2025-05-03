using Common.Objects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers;

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
            _carDetails = GameManager.Instance.InstantiatedCar.GetComponent<CarDetails>();
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
                var renderer = part.GetComponent<Renderer>();
                renderer.material = new Material(_colours[index]);
            }
        }

    }
}
