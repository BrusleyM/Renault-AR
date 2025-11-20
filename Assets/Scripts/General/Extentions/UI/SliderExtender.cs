using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


namespace Extensions.UI
{
    public static class SliderExtender
    {
        /// <summary>
        /// Binds the slider to a TMP_Text or legacy Text component to display its value.
        /// </summary>
        public static void ExtendSlider(this Slider slider, Component textComponent,string format)
        {
            if (slider == null || textComponent == null)
            {
                Debug.LogWarning("Slider or Text Component is null");
                return;
            }

            Action<float> updateText = null;
            if (textComponent is TMP_Text tmpText)
            {
                updateText = value => tmpText.text = string.Format(format, value);
            }
            else if (textComponent is Text legacyText)
            {
                updateText = value => legacyText.text = string.Format(format, value);
            }
            else
            {
                Debug.LogError("Unsupported text component type");
                return;
            }

            slider.onValueChanged.AddListener(v => updateText(v));
            updateText(slider.value);
        }
    }
}