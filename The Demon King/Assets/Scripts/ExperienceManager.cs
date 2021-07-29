using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ExperienceManager : MonoBehaviour
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
            if (red.expBar.CurrentExp >= red.expBar.level1ExpNeeded)
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
            if (green.expBar.CurrentExp >= green.expBar.level1ExpNeeded)
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
            if (blue.expBar.CurrentExp >= blue.expBar.level1ExpNeeded)
            {
                NextEvolution = blue.evo1;
                red.CanEvolve = false;
                blue.CanEvolve = true;
                green.CanEvolve = false;
            }
        }
    }
}
