using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ExperienceBar
{
    [Header("EXP SLIDER FOR MY TYPE")]


    public GameObject ActiveExpBarBackground;
    //public GameObject ActiveExpBarCanEvolveTxt;
    [HideInInspector] public Material expMaterialCopy;
    public Material expMaterial;
    public Image fillImage;
    public GameObject expThreshholdBar;
    public GameObject childDisplayImg;
    public GameObject adultDisplayImg;



    public float CurrentExp;

    [Header("EXPERIENCE REQUIRED TO EVOLVE")]
    public FloatSO level1ExpNeeded;
    public FloatSO level2ExpNeeded;




    //Upates exp bar slider to current 
    public void UpdateExpBar(float CurrentExp)
    {
        expMaterialCopy.SetFloat("_CurrentHealth", CurrentExp);
    }
}
