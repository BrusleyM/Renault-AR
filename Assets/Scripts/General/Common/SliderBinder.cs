using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Extensions.UI;

namespace Common
{
    public class SliderBinder : MonoBehaviour
    {
        public Slider mySlider;
        public TMP_Text valueText;
        [SerializeField, Tooltip("Use format like: 'Value: {0:F1}'")]
        string format;

        void Start()
        {
            mySlider.ExtendSlider(valueText,format);
        }
    }
}
