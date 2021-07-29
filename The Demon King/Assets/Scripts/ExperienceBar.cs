using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ExperienceBar
{
    public Slider expSlider;

    public int CurrentExp;
    public int MaxExp;

    public int level1ExpNeeded;
    public int level2ExpNeeded;

    public void UpdateSlider()
    {

    }
}
