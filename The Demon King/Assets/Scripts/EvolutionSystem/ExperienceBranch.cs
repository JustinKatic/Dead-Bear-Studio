using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExperienceBranch
{
    [Header("Evolutions")] 
    [HideInInspector] public string startingEvo = "StartingEvo";
    [HideInInspector] public string evo1 = "EvolutionOne";
    [HideInInspector] public string evo2 = "EvolutionTwo";
    [HideInInspector] public string branchName;

    [HideInInspector] public bool CanEvolve = false;
    public ExperienceBar expBar;
}
