using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerLeaderboardPanel : MonoBehaviour
{
    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI DemonKingScoreText;
    public Image FillImg;
    public SOMenuData roomData;
    public Image CurrentEvolutionImg;
    public GameObject NearWinVFX;

    private void Awake()
    {
        FillImg.fillAmount = 0;
    }

    public void UpdateSliderValue(int NewValue)
    {
        FillImg.fillAmount = NewValue / (float)roomData.PointsToWin;
    }
}
