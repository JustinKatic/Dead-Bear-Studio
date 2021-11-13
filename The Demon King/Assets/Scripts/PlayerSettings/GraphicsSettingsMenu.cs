using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GraphicsSettingsMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text FPSText;
    [SerializeField] private Slider FPSSlider;
    [SerializeField] private RenderPipelineAsset[] Quality;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = PlayerPrefs.GetInt("FramesPerSecond", 300);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 2));
        QualitySettings.renderPipeline = Quality[PlayerPrefs.GetInt("Quality", 2)];

        FPSSlider.minValue = 60f;
        FPSSlider.maxValue = 300f;

        FPSSlider.value = Application.targetFrameRate;
        FPSText.text = FPSSlider.value.ToString();
    }

    public void OnFramesSliderChange(Slider newFPSValue)
    {
        PlayerPrefs.SetInt("FramesPerSecond", (int)newFPSValue.value);
        FPSText.text = newFPSValue.value.ToString();
        Application.targetFrameRate = (int)newFPSValue.value;
    }


    public void SetQualityLow()
    {
        QualitySettings.SetQualityLevel(0);
        QualitySettings.renderPipeline = Quality[0];
        PlayerPrefs.SetInt("Quality", 0);
    }
    public void SetQualityMedium()
    {
        QualitySettings.SetQualityLevel(1);
        QualitySettings.renderPipeline = Quality[1];
        PlayerPrefs.SetInt("Quality", 1);
    }
    public void SetQualityHigh()
    {
        QualitySettings.SetQualityLevel(2);
        QualitySettings.renderPipeline = Quality[2];
        PlayerPrefs.SetInt("Quality", 2);
    }
}
