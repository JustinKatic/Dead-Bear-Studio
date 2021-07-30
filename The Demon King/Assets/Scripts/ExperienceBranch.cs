using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExperienceBranch
{

    public Evolutions evo1;
    public Evolutions evo2;

    public bool CanEvolve = false;

    public ExperienceBar expBar;
}
