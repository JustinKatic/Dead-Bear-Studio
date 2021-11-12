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
    private Material material;
    public GameObject NearWinVFX;

    private void Awake()
    {
        material = Instantiate(FillImg.material);
        FillImg.material = material;

        FillImg.material.SetFloat("_MaxHealth", roomData.PointsToWin);
        FillImg.material.SetFloat("_CurrentHealth", 0);
        FillImg.fillAmount = 1;
    }

    public void UpdateSliderValue(int NewValue)
    {
        FillImg.material.SetFloat("_CurrentHealth", NewValue);
    }
}
