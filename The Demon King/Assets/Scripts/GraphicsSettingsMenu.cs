using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsMenu : MonoBehaviour
{
    [SerializeField]private TMP_Text FPSText;
    [SerializeField]private Slider FPSSlider;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt("FramesPerSecond",300);
        FPSSlider.value = Application.targetFrameRate;
        FPSText.text = FPSSlider.value.ToString();
        FPSSlider.onValueChanged.AddListener(OnFramesSliderChange);
    }

    private void OnFramesSliderChange(float newFPSValue)
    {
        PlayerPrefs.SetInt("FramesPerSecond", (int)newFPSValue);
        FPSText.text = newFPSValue.ToString();

        Application.targetFrameRate = (int)newFPSValue;
    }
    
}
