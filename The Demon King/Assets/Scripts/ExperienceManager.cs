using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class ExperienceManager : MonoBehaviourPun
{
    public Evolutions CurrentEvolution;
    public MinionType CurrentMinionType;
    [HideInInspector] public Evolutions NextEvolution;

    public ExperienceBranch green;
    public ExperienceBranch red;
    public ExperienceBranch blue;


    public MinionType redMinion;
    public MinionType greenMinion;
    public MinionType blueMinion;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            red.expBar.expSlider.maxValue = red.expBar.MaxExp;
            red.expBar.expSlider.value = red.expBar.CurrentExp;

            blue.expBar.expSlider.maxValue = blue.expBar.MaxExp;
            blue.expBar.expSlider.value = blue.expBar.CurrentExp;

            green.expBar.expSlider.maxValue = green.expBar.MaxExp;
            green.expBar.expSlider.value = green.expBar.CurrentExp;
        }
    }


    public bool CanEvolve()
    {
        if (red.CanEvolve || blue.CanEvolve || green.CanEvolve)
        {
            red.CanEvolve = false;
            blue.CanEvolve = false;
            green.CanEvolve = false;
            return true;
        }
        return false;
    }


    public void AddExpereince(MinionType minionType, int expValue)
    {
        if (minionType == redMinion)
        {
            red.expBar.CurrentExp = Mathf.Clamp(red.expBar.CurrentExp + expValue, 0, red.expBar.MaxExp);
            red.expBar.UpdateExpSlider();
            if (red.expBar.CurrentExp >= red.expBar.level2ExpNeeded)
            {
                NextEvolution = red.evo2;
                red.CanEvolve = true;
                blue.CanEvolve = false;
                green.CanEvolve = false;
            }
            else if (red.expBar.CurrentExp >= red.expBar.level1ExpNeeded)
            {
                NextEvolution = red.evo1;
                red.CanEvolve = true;
                blue.CanEvolve = false;
                green.CanEvolve = false;
            }
        }
        else if (minionType == greenMinion)
        {
            green.expBar.CurrentExp = Mathf.Clamp(green.expBar.CurrentExp + expValue, 0, green.expBar.MaxExp);
            green.expBar.UpdateExpSlider();
            if (green.expBar.CurrentExp >= green.expBar.level2ExpNeeded)
            {
                NextEvolution = green.evo2;
                red.CanEvolve = false;
                blue.CanEvolve = false;
                green.CanEvolve = true;
            }
            else if (green.expBar.CurrentExp >= green.expBar.level1ExpNeeded)
            {
                NextEvolution = green.evo1;
                red.CanEvolve = false;
                blue.CanEvolve = false;
                green.CanEvolve = true;
            }
        }
        else if (minionType == blueMinion)
        {
            blue.expBar.CurrentExp = Mathf.Clamp(blue.expBar.CurrentExp + expValue, 0, blue.expBar.MaxExp);
            blue.expBar.UpdateExpSlider();
            if (blue.expBar.CurrentExp >= blue.expBar.level2ExpNeeded)
            {
                NextEvolution = blue.evo2;
                red.CanEvolve = false;
                blue.CanEvolve = true;
                green.CanEvolve = false;
            }
            else if (blue.expBar.CurrentExp >= blue.expBar.level1ExpNeeded)
            {
                NextEvolution = blue.evo1;
                red.CanEvolve = false;
                blue.CanEvolve = true;
                green.CanEvolve = false;
            }
        }
    }
}
