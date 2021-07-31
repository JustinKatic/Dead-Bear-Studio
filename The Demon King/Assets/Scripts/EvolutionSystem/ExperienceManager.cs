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
            SetSliders();
        }
    }
    //This runs inside the evolution Manager when the evolution button has been pressed
    public bool CanEvolve()
    {
        //Check if I can evolve into any of these types
        if (red.CanEvolve || blue.CanEvolve || green.CanEvolve)
        {
            //Player has evolved, can not evolve again
            red.CanEvolve = false;
            blue.CanEvolve = false;
            green.CanEvolve = false;
            return true;
        }
        return false;
    }
    //Add the experience to the correct Branch type
    public void AddExpereince(MinionType minionType, int expValue)
    {
        if (minionType == redMinion)
        {
            UpdateBranchType(red, expValue);

        }
        else if (minionType == greenMinion)
        {
            UpdateBranchType(green, expValue);

        }
        else if (minionType == blueMinion)
        {
            UpdateBranchType(blue, expValue);
           
        }
    }

    void SetSliders()
    {
        red.expBar.expSlider.maxValue = red.expBar.MaxExp;
        red.expBar.expSlider.value = red.expBar.CurrentExp;

        blue.expBar.expSlider.maxValue = blue.expBar.MaxExp;
        blue.expBar.expSlider.value = blue.expBar.CurrentExp;

        green.expBar.expSlider.maxValue = green.expBar.MaxExp;
        green.expBar.expSlider.value = green.expBar.CurrentExp;
    }

    void ChangeEvolutionBools(bool redBool, bool greenBool, bool blueBool)
    {
        red.CanEvolve = redBool;
        green.CanEvolve = greenBool;
        blue.CanEvolve = blueBool;
    }
    //Update the correct branch based off the given parameters
    void UpdateBranchType(ExperienceBranch branchType, int value)
    {
        branchType.expBar.CurrentExp = Mathf.Clamp(branchType.expBar.CurrentExp + value, 0, branchType.expBar.MaxExp);
        branchType.expBar.UpdateExpSlider();
            
        if (branchType.expBar.CurrentExp >= branchType.expBar.level2ExpNeeded)
        {
            NextEvolution = branchType.evo2;
            ChangeEvolutionBools(true, false, false);

        }
        else if (branchType.expBar.CurrentExp >= branchType.expBar.level1ExpNeeded)
        {
            NextEvolution = branchType.evo1;
            ChangeEvolutionBools(true, false, false);
        }
    }
}
