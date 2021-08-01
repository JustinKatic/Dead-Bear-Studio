using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ExperienceManager : MonoBehaviourPun
{
    [HideInInspector] public MinionType CurrentMinionType;
    [HideInInspector] public string NextEvolution;
    [HideInInspector] public string CurrentEvolution;
    
    public ExperienceBranch green;
    public ExperienceBranch red;
    public ExperienceBranch blue;

    public MinionType redMinion;
    public MinionType greenMinion;
    public MinionType blueMinion;

    private EvolutionManager evolutionManager;

    private void Awake()
    {
        red.branchName = "Red";
        green.branchName = "Green";
        blue.branchName = "Blue";
        
        if (photonView.IsMine)
        {
            evolutionManager = GetComponent<EvolutionManager>();
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
            UpdateBranchType(red, expValue, true);
        }
        else if (minionType == greenMinion)
        {
            UpdateBranchType(green, expValue, true);

        }
        else if (minionType == blueMinion)
        {
            UpdateBranchType(blue, expValue, true);
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

    void ChangeEvolutionBools(ExperienceBranch branch)
    {
        if (branch == red)
        {
            red.CanEvolve = true;
            green.CanEvolve = false;
            blue.CanEvolve = false;
        }
        if (branch == green)
        {
            red.CanEvolve = false;
            green.CanEvolve = true;
            blue.CanEvolve = false;
        }
        if (branch == blue)
        {
            red.CanEvolve = false;
            green.CanEvolve = false;
            blue.CanEvolve = true;
        }
    }
    //Update the correct branch based off the given parameters
    void UpdateBranchType(ExperienceBranch branchType, int value, bool experienceIncrease)
    {
        if (experienceIncrease)
        {
            branchType.expBar.CurrentExp = Mathf.Clamp(branchType.expBar.CurrentExp + value, 0, branchType.expBar.MaxExp);
            branchType.expBar.UpdateExpSlider();

            // if experience is greater than level 2
            if (branchType.expBar.CurrentExp >= branchType.expBar.level2ExpNeeded)
            {
                if (CurrentEvolution != branchType.evo2)
                {
                    NextEvolution = String.Concat(branchType.branchName, branchType.evo2);
                    ChangeEvolutionBools(branchType);
                }
            }
            // if experience is greater than level 1
            else if (branchType.expBar.CurrentExp >= branchType.expBar.level1ExpNeeded)
            {
                if (CurrentEvolution != branchType.evo1)
                {
                    NextEvolution = String.Concat(branchType.branchName, branchType.evo1);
                    ChangeEvolutionBools(branchType);
                }
            }
        }
        else
        {
            branchType.expBar.CurrentExp = Mathf.Clamp(branchType.expBar.CurrentExp - value, 0, branchType.expBar.MaxExp);
            branchType.expBar.UpdateExpSlider();

            //Current exp is less then level 1 required
            if (branchType.expBar.CurrentExp < branchType.expBar.level1ExpNeeded)
            {
                //Will need to be changed to Concat once All oozes added
                NextEvolution = branchType.startingEvo;
                evolutionManager.ChangeEvolution(NextEvolution);
            }
            //Current exp is less then level 2 required
            else if (branchType.expBar.CurrentExp < branchType.expBar.level2ExpNeeded)
            {
                NextEvolution = String.Concat(branchType.branchName, branchType.evo1);
                evolutionManager.ChangeEvolution(NextEvolution);
            }
        }
    }

    public void CheckEvolutionOnDeath(MinionType currentMinionType, int experienceLoss)
    {
        if (currentMinionType == redMinion)
        {
            UpdateBranchType(red, experienceLoss, false);
        }
        else if (currentMinionType == greenMinion)
        {
            UpdateBranchType(green, experienceLoss, false);
        }
        else if (currentMinionType == blueMinion)
        {
            UpdateBranchType(blue, experienceLoss, false);
        }
    }
}
