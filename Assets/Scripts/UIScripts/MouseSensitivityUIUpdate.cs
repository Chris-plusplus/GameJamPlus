using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivityUIUpdate : MonoBehaviour
{
    public Slider mouseSensitivitySlider;
    public Text text;

    public void UpdateSlider()
    {
        text.text = (Mathf.Round(mouseSensitivitySlider.value * 100f)/100f).ToString();
    }
}
