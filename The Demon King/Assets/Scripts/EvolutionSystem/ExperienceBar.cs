using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ExperienceBar
{
    public Slider expSlider;

    public int CurrentExp;
    
    [Header("Experience for Evolutions")]
    public int MaxExp;
    public int level1ExpNeeded;
    public int level2ExpNeeded;

    
    public void UpdateExpSlider()
    {
        expSlider.value = CurrentExp;
    }
}
