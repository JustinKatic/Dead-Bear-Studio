using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ExperienceBar
{
    [Header("EXP SLIDER FOR MY TYPE")]
    public Slider expSlider;

    public GameObject ActiveExpBarBackground;
    public GameObject ActiveExpBarCanEvolveTxt;




    public float CurrentExp;

    [Header("EXPERIENCE REQUIRED TO EVOLVE")]
    public IntSO level1ExpNeeded;
    public IntSO level2ExpNeeded;


    //Upates exp bar slider to current 
    public void UpdateExpSlider()
    {
        expSlider.value = CurrentExp;
    }
}
