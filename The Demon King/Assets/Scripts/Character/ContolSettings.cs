using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContolSettings : MonoBehaviourPun
{
    private PlayerController controller;
    
    [SerializeField] private TMP_Text SensitivityValue;

    [SerializeField] private Slider MouseSlider;
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            controller = GetComponentInParent<PlayerController>();
            MouseSlider.maxValue = 100;
            MouseSlider.value = PlayerPrefs.GetFloat("MouseSensitivity", controller.MouseSensitivity);
            SensitivityValue.text = MouseSlider.value.ToString("F1");
        }
    }

    public void OnChangeMouseSensitivity(Slider mouseSensitivity)
    {
        if (controller != null)
        {
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity.value);
            controller.MouseSensitivity = mouseSensitivity.value;
            SensitivityValue.text = controller.MouseSensitivity.ToString("F1");
        }
    }
}
