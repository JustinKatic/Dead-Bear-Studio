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
    public GameObject displayImg;
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

    ////Upates exp bar slider to current 
    //public void UpdateExpBar(float CurrentExp, float prevExp)
    //{
    //    expMaterialCopy.SetFloat("_CurrentHealth", CurrentExp);
    //}

    public IEnumerator UpdateExpBar(float CurrentExp, float prevExp)
    {
        float lerpVal = prevExp;
        while (Mathf.Abs(lerpVal - CurrentExp) > 0.05)
        {
            lerpVal = Mathf.Lerp(lerpVal, CurrentExp, 5 * Time.deltaTime);
            expMaterialCopy.SetFloat("_CurrentHealth", lerpVal);
            yield return null;
        }
        expMaterialCopy.SetFloat("_CurrentHealth", CurrentExp);
    }
}
