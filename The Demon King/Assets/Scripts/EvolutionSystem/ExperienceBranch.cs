using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExperienceBranch
{
    [Header("Evolutions")] 
    public Evolutions startingEvo;
    public Evolutions evo1;
    public Evolutions evo2;

    [HideInInspector] public bool CanEvolve = false;
    public ExperienceBar expBar;
}
