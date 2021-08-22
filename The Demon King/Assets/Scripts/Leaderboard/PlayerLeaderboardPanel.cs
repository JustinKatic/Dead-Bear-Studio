using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerLeaderboardPanel : MonoBehaviour
{
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI TimeSpentAsDemonKing;
    private Slider slider;
    public IntSO TimeRequiredToWin;
    public Image CurrentEvolutionImg;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = TimeRequiredToWin.value;
        slider.value = 0;
    }

    public void UpdateSliderValue(int NewValue)
    {
        if (slider != null)
            slider.value = NewValue;
    }
}
