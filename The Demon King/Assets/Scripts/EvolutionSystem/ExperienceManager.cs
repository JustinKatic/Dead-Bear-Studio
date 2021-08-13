using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class ExperienceManager : MonoBehaviourPun
{
    [Header("Modifible stats")]
    public float ScaleAmount = 0.1f;
    public float CamDistanceIncreaseAmount = .5f;
    public float CamShoulderOffsetXIncreaseAmount = .1f;
    public float PercentOfExpToLoseOnDeath = .20f;
    public float DemonKingExpLossDeath = 0.4f;

    [Header("EVOLUTION TYPES")]
    public ExperienceBranch green;
    public ExperienceBranch red;
    public ExperienceBranch blue;

    [Header("MINION TYPES")]
    public MinionType redMinion;
    public MinionType greenMinion;
    public MinionType blueMinion;

    private EvolutionManager evolutionManager;
    private DemonKingEvolution demonKingEvolution;

    private Vector3 BaseScale;
    private float baseCamDist;
    private float baseCamShoulderX;

    [HideInInspector] public ExperienceBranch CurrentActiveEvolutionBranch;

    private List<MinionType> minionTypes = new List<MinionType>();

    private Cinemachine3rdPersonFollow vCam;

    private HealthManager healthManager;


    private void Awake()
    {
        healthManager = GetComponent<HealthManager>();

        //If local
        minionTypes.Add(greenMinion);
        minionTypes.Add(redMinion);
        minionTypes.Add(blueMinion);

        if (photonView.IsMine)
        {
            SetMyMinionTypeOnStart();
            vCam = gameObject.GetComponentInChildren<Cinemachine3rdPersonFollow>();
            BaseScale = transform.localScale;
            baseCamDist = vCam.CameraDistance;
            baseCamShoulderX = vCam.ShoulderOffset.x;

            evolutionManager = GetComponent<EvolutionManager>();
            demonKingEvolution = GetComponent<DemonKingEvolution>();

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
        // if (healthManager.MyMinionType != null)
        healthManager.MyMinionType = minionTypes[minionType];
    }

    void SetStartingActiveEvolution()
    {
        if (healthManager.MyMinionType == redMinion)
        {
            SetStartBranchType(red);
        }
        else if (healthManager.MyMinionType == blueMinion)
        {
            SetStartBranchType(blue);
        }
        else if (healthManager.MyMinionType == greenMinion)
        {
            SetStartBranchType(green);
        }
    }

    void SetStartBranchType(ExperienceBranch branchType)
    {
        evolutionManager.activeEvolution = branchType.Level0Evolution;
        evolutionManager.nextBranchType = branchType;
        evolutionManager.nextEvolution = branchType.Level0Evolution;
        branchType.ExpBar.ActiveExpBarBackground.SetActive(true);
    }

    void UpdateActiveBranchUI(ExperienceBranch branchType)
    {
        if (branchType == red)
        {
            red.ExpBar.ActiveExpBarBackground.SetActive(true);
            green.ExpBar.ActiveExpBarBackground.SetActive(false);
            blue.ExpBar.ActiveExpBarBackground.SetActive(false);
        }
        else if (branchType == green)
        {
            red.ExpBar.ActiveExpBarBackground.SetActive(false);
            green.ExpBar.ActiveExpBarBackground.SetActive(true);
            blue.ExpBar.ActiveExpBarBackground.SetActive(false);
        }
        else if (branchType == blue)
        {
            red.ExpBar.ActiveExpBarBackground.SetActive(false);
            green.ExpBar.ActiveExpBarBackground.SetActive(false);
            blue.ExpBar.ActiveExpBarBackground.SetActive(true);
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
        {
            vCam.CameraDistance = baseCamDist + CurrentExp * CamDistanceIncreaseAmount;
            vCam.ShoulderOffset.x = baseCamShoulderX + CurrentExp * CamShoulderOffsetXIncreaseAmount;
        }
        else
        {
            vCam.CameraDistance = baseCamDist;
            vCam.ShoulderOffset.x = baseCamShoulderX;
        }
    }


    //Update the correct branch based off the given parameters
    void UpdateBranchType(ExperienceBranch branchType, int value)
    {
        branchType.ExpBar.CurrentExp = Mathf.Clamp(branchType.ExpBar.CurrentExp + value, 0, branchType.ExpBar.level2ExpNeeded.value);
        branchType.ExpBar.UpdateExpSlider();

        UpdateActiveBranchUI(branchType);

        if (branchType == CurrentActiveEvolutionBranch && !demonKingEvolution.AmITheDemonKing)
            ScaleSize(branchType.ExpBar.CurrentExp);

        //if (branchType.Level0Evolution.MyMinionType == evolutionManager.activeEvolution.MyMinionType)
        //ScaleSize(branchType.ExpBar.CurrentExp);

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

    public void DecreaseExperince(float decreaseValue)
    {
        UpdateExpBarOnDecrease(red, decreaseValue);
        UpdateExpBarOnDecrease(green, decreaseValue);
        UpdateExpBarOnDecrease(blue, decreaseValue);
    }

    public void CheckIfNeedToDevolve()
    {
        if (CurrentActiveEvolutionBranch == red)
        {
            DevolveIfExpDroppedBelowThreshold(red);
        }

        else if (CurrentActiveEvolutionBranch == green)
        {
            DevolveIfExpDroppedBelowThreshold(green);
        }

        else if (CurrentActiveEvolutionBranch == blue)
        {
            DevolveIfExpDroppedBelowThreshold(blue);
        }
    }


    void UpdateExpBarOnDecrease(ExperienceBranch branchToUpdate, float decreaseValue)
    {
        branchToUpdate.ExpBar.CurrentExp = Mathf.Clamp(branchToUpdate.ExpBar.CurrentExp - (branchToUpdate.ExpBar.CurrentExp * decreaseValue), 0, branchToUpdate.ExpBar.level2ExpNeeded.value);
        branchToUpdate.ExpBar.UpdateExpSlider();
    }

    void DevolveIfExpDroppedBelowThreshold(ExperienceBranch CurrentActiveEvolutionBranch)
    {
        ExperienceBar currentExpBar = CurrentActiveEvolutionBranch.ExpBar;

        ScaleSize(currentExpBar.CurrentExp);
        //If exp is less then level 1 && If current active evlolution lvl 0 gameobject is false
        if (currentExpBar.CurrentExp < currentExpBar.level1ExpNeeded.value && evolutionManager.activeEvolution != CurrentActiveEvolutionBranch.Level0Evolution)
        {
            evolutionManager.nextEvolution = CurrentActiveEvolutionBranch.Level0Evolution;
            evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
        }

        else if (currentExpBar.CurrentExp < currentExpBar.level2ExpNeeded.value && currentExpBar.CurrentExp >= currentExpBar.level1ExpNeeded.value
            && evolutionManager.activeEvolution != CurrentActiveEvolutionBranch.Level1Evolution)
        {
            evolutionManager.nextEvolution = CurrentActiveEvolutionBranch.Level1Evolution;
            evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
        }
    }

    //This function is called by DemonKingEvolution script
    public void ActivateDemonKingEvolution()
    {
        evolutionManager.nextEvolution = CurrentActiveEvolutionBranch.DemonKingEvolution;
        evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, false);
        ScaleSize(CurrentActiveEvolutionBranch.ExpBar.level2ExpNeeded.value);
    }

}
