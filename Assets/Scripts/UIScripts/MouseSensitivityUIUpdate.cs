using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivityUIUpdate : MonoBehaviour
{
    private const string mouseSensitivityKey = "mouseSensitivity";

    public Slider mouseSensitivitySlider;
    public Text text;

    private void Start()
    {
        mouseSensitivitySlider.value = SettingsMenu.MouseSensitivity;
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        PlayerPrefs.SetFloat(mouseSensitivityKey, mouseSensitivitySlider.value);
        text.text = (Mathf.Round(mouseSensitivitySlider.value * 100f)/100f).ToString();
    }
}
