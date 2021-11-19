using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControlSettingsMenu : MonoBehaviour
{
    private Slider sensitivitySlider;
    [SerializeField] private TMP_Text sliderText;

    private void Start()
    {
        sensitivitySlider = GetComponent<Slider>();
        sensitivitySlider.maxValue = 100;
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 10);
        sliderText.text = PlayerPrefs.GetFloat("MouseSensitivity", 10).ToString();
    }

    public void OnMouseSensitivityChange(Slider SensitivitySlider)
    {
        sliderText.text = SensitivitySlider.value.ToString();

        PlayerPrefs.SetFloat("MouseSensitivity", SensitivitySlider.value);
    }
}
