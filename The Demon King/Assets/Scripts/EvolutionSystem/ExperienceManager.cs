using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviourPun
{
    [Header("Modifible stats")]
    public float ScaleAmount = 0.1f;
    public float CamDistanceIncreaseAmount = .5f;
    public float PercentOfExpToLoseOnDeath = .20f;


    [Header("EVOLUTION TYPES")]
    public ExperienceBranch green;
    public ExperienceBranch red;
    public ExperienceBranch blue;

    [Header("MINION TYPES")]
    public MinionType redMinion;
    public MinionType greenMinion;
    public MinionType blueMinion;

    private EvolutionManager evolutionManager;

    private Vector3 BaseScale;
    private float baseCamDist;

    [HideInInspector] public ExperienceBranch currentBranch;

    [SerializeField] private List<MinionType> minionTypes = new List<MinionType>();

    private Cinemachine3rdPersonFollow vCam;

    private HealthManager healthManager;




    private void Awake()
    {
        healthManager = GetComponent<HealthManager>();
        //If local
        if (photonView.IsMine)
        {
            SetMyMinionTypeOnStart();
            vCam = gameObject.GetComponentInChildren<Cinemachine3rdPersonFollow>();
            BaseScale = transform.localScale;
            baseCamDist = vCam.CameraDistance;

            evolutionManager = GetComponent<EvolutionManager>();
            SetSliders();
            SetStartingActiveEvolution();
        }
    }

    private void Start()
    {
        if (photonView.IsMine)
            evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
    }

    private void SetMyMinionTypeOnStart()
    {
        //Get a random location
        int randomMinionType = Random.Range(0, minionTypes.Count);
        photonView.RPC("SetMinionType", RpcTarget.All, randomMinionType);
    }

    [PunRPC]
    void SetMinionType(int minionType)
    {
        healthManager.MyMinionType = minionTypes[minionType];
    }

    void SetStartingActiveEvolution()
    {
        if (healthManager.MyMinionType == redMinion)
        {
            evolutionManager.activeEvolution = red.Level0Evolution;
            evolutionManager.nextBranchType = red;
            evolutionManager.nextEvolution = red.Level0Evolution;
            red.ExpBar.ActiveExpBarBackground.SetActive(true);
            red.ExpBar.UnActiveExpBarBackground.SetActive(false);
        }
        else if (healthManager.MyMinionType == blueMinion)
        {
            evolutionManager.activeEvolution = blue.Level0Evolution;
            evolutionManager.nextBranchType = blue;
            evolutionManager.nextEvolution = blue.Level0Evolution;
            blue.ExpBar.ActiveExpBarBackground.SetActive(true);
            blue.ExpBar.UnActiveExpBarBackground.SetActive(false);
        }
        else if (healthManager.MyMinionType == greenMinion)
        {
            evolutionManager.activeEvolution = green.Level0Evolution;
            evolutionManager.nextBranchType = green;
            evolutionManager.nextEvolution = green.Level0Evolution;
            green.ExpBar.ActiveExpBarBackground.SetActive(true);
            green.ExpBar.UnActiveExpBarBackground.SetActive(false);
        }
    }

    //This runs inside the evolution Manager when the evolution button has been pressed
    public bool CanEvolve()
    {
        //Check if I can evolve into any of these types is yes reset can evolve and return true else return false
        if (red.CanEvolve || blue.CanEvolve || green.CanEvolve)
        {
            return true;
        }
        return false;
    }

    public void SetCanEvolveFalse()
    {
        //Player has evolved, can not evolve again
        red.CanEvolve = false;
        blue.CanEvolve = false;
        green.CanEvolve = false;
    }


    //Set sliders max and current values called locally in Awake()
    void SetSliders()
    {
        red.ExpBar.expSlider.maxValue = red.ExpBar.level2ExpNeeded.value;
        red.ExpBar.expSlider.value = red.ExpBar.CurrentExp;

        blue.ExpBar.expSlider.maxValue = blue.ExpBar.level2ExpNeeded.value;
        blue.ExpBar.expSlider.value = blue.ExpBar.CurrentExp;

        green.ExpBar.expSlider.maxValue = green.ExpBar.level2ExpNeeded.value;
        green.ExpBar.expSlider.value = green.ExpBar.CurrentExp;
    }



    //Sets can evolve based of branch type passed in
    public void ChangeEvolutionBools(ExperienceBranch branch)
    {
        if (branch == red)
        {
            red.CanEvolve = true;
            green.CanEvolve = false;
            blue.CanEvolve = false;
        }
        else if (branch == green)
        {
            red.CanEvolve = false;
            green.CanEvolve = true;
            blue.CanEvolve = false;
        }
        else if (branch == blue)
        {
            red.CanEvolve = false;
            green.CanEvolve = false;
            blue.CanEvolve = true;
        }
    }

    public void ScaleSize(float CurrentExp)
    {
        transform.localScale = BaseScale + Vector3.one * CurrentExp * ScaleAmount;
        if (CurrentExp >= 1)
            vCam.CameraDistance = baseCamDist + CurrentExp * CamDistanceIncreaseAmount;
    }


    //Update the correct branch based off the given parameters
    void UpdateBranchType(ExperienceBranch branchType, int value)
    {
        branchType.ExpBar.CurrentExp = Mathf.Clamp(branchType.ExpBar.CurrentExp + value, 0, branchType.ExpBar.level2ExpNeeded.value);
        branchType.ExpBar.UpdateExpSlider();

        if (branchType == currentBranch)
            ScaleSize(branchType.ExpBar.CurrentExp);

        if (branchType.Level0Evolution.MyMinionType == evolutionManager.activeEvolution.MyMinionType)
            ScaleSize(branchType.ExpBar.CurrentExp);

        // if experience is greater than level 2
        if (branchType.ExpBar.CurrentExp >= branchType.ExpBar.level2ExpNeeded.value)
        {
            if (evolutionManager.activeEvolution != branchType.Level2Evolution)
            {
                evolutionManager.nextEvolution = branchType.Level2Evolution;
                evolutionManager.nextBranchType = branchType;
                ChangeEvolutionBools(branchType);
            }
        }
        // if experience is greater than level 1
        else if (branchType.ExpBar.CurrentExp >= branchType.ExpBar.level1ExpNeeded.value)
        {
            if (evolutionManager.activeEvolution != branchType.Level1Evolution)
            {
                evolutionManager.nextEvolution = branchType.Level1Evolution;
                evolutionManager.nextBranchType = branchType;
                ChangeEvolutionBools(branchType);
            }
        }
    }

    //Add the experience based of minion type eaten
    public void AddExpereince(MinionType minionType, int expValue)
    {
        //update red exp
        if (minionType == redMinion)
        {
            UpdateBranchType(red, expValue);
        }
        //update green exp
        else if (minionType == greenMinion)
        {
            UpdateBranchType(green, expValue);
        }
        //update blue exp
        else if (minionType == blueMinion)
        {
            UpdateBranchType(blue, expValue);
        }
    }

    public void DecreaseExperince()
    {
        red.ExpBar.CurrentExp = Mathf.Clamp(red.ExpBar.CurrentExp - (red.ExpBar.CurrentExp * PercentOfExpToLoseOnDeath), 0, red.ExpBar.level2ExpNeeded.value);
        red.ExpBar.UpdateExpSlider();

        green.ExpBar.CurrentExp = Mathf.Clamp(green.ExpBar.CurrentExp - (green.ExpBar.CurrentExp * PercentOfExpToLoseOnDeath), 0, green.ExpBar.level2ExpNeeded.value);
        green.ExpBar.UpdateExpSlider();

        blue.ExpBar.CurrentExp = Mathf.Clamp(blue.ExpBar.CurrentExp - (blue.ExpBar.CurrentExp * PercentOfExpToLoseOnDeath), 0, blue.ExpBar.level2ExpNeeded.value);
        blue.ExpBar.UpdateExpSlider();


        if (currentBranch == red)
        {
            ScaleSize(red.ExpBar.CurrentExp);
            if (red.ExpBar.CurrentExp < red.ExpBar.level1ExpNeeded.value)
            {
                evolutionManager.nextEvolution = red.Level0Evolution;
                evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
            }
            else if (red.ExpBar.CurrentExp < red.ExpBar.level2ExpNeeded.value)
            {
                evolutionManager.nextEvolution = red.Level1Evolution;
                evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
            }
        }

        else if (currentBranch == green)
        {
            ScaleSize(green.ExpBar.CurrentExp);
            if (green.ExpBar.CurrentExp < green.ExpBar.level1ExpNeeded.value)
            {
                evolutionManager.nextEvolution = green.Level0Evolution;
                evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
            }
            else if (green.ExpBar.CurrentExp < green.ExpBar.level2ExpNeeded.value)
            {
                evolutionManager.nextEvolution = green.Level1Evolution;
                evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
            }
        }

        else if (currentBranch == blue)
        {
            ScaleSize(blue.ExpBar.CurrentExp);
            if (blue.ExpBar.CurrentExp < blue.ExpBar.level1ExpNeeded.value)
            {
                evolutionManager.nextEvolution = blue.Level0Evolution;
                evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
            }
            else if (blue.ExpBar.CurrentExp < blue.ExpBar.level2ExpNeeded.value)
            {
                evolutionManager.nextEvolution = blue.Level1Evolution;
                evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
            }
        }
    }
}
