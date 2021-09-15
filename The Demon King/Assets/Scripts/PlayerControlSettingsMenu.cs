using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlSettingsMenu : MonoBehaviour
{
    private Slider sensitivitySlider;
    private void Start()
    {
        sensitivitySlider = GetComponent<Slider>();
        sensitivitySlider.maxValue = 100;
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 10);
    }

    public void OnMouseSensitivityChange(Slider SensitivitySlider)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", SensitivitySlider.value);
    }
}
