using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ExperienceBar
{
    [Header("EXP SLIDER FOR MY TYPE")]


    public GameObject ActiveExpBarBackground;
    public GameObject ActiveExpBarCanEvolveTxt;
    [HideInInspector] public Material expMaterialCopy;
    public Material expMaterial;
    public Image fillImage;
    public GameObject expThreshholdBar;
    public GameObject childDisplayImg;
    public GameObject adultDisplayImg;
    public GameObject expBarParent;



    public float CurrentExp;

    [Header("EXPERIENCE REQUIRED TO EVOLVE")]
    public FloatSO level1ExpNeeded;
    public FloatSO level2ExpNeeded;


    public void UpdateActiveExpBarCanEvolveText()
    {
        string path = InputManager.inputActions.Player.Evolve.bindings[0].path;
        string key = path.Split('/').Last();
        ActiveExpBarCanEvolveTxt.GetComponentInChildren<TextMeshProUGUI>().text = key;
    }

    //Upates exp bar slider to current 
    public void UpdateExpBar(float CurrentExp)
    {
        expMaterialCopy.SetFloat("_CurrentHealth", CurrentExp);
    }
}
