using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExperienceBranch
{
    [Header("MY EVOLUTIONS")]
    public Evolutions Level0Evolution;
    public Evolutions Level1Evolution;
    public Evolutions Level2Evolution;
    public Evolutions DemonKingEvolution;

    [HideInInspector] public bool CanEvolve = false;
    public ExperienceBar ExpBar;
    public MinionType branchMinionType;

}
