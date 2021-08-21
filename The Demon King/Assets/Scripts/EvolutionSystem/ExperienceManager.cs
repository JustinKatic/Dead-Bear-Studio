using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class ExperienceManager : MonoBehaviourPun
{
    [Header("Modifible stats")]
    [SerializeField] private float ScaleAmount = 0.1f;
    [SerializeField] private float CamDistanceIncreaseAmount = .5f;
    [SerializeField] private float CamShoulderOffsetXIncreaseAmount = 0f;
    [SerializeField] private float CamShoulderOffsetYIncreaseAmount = 0f;

    [SerializeField] private float percentOfExpToLoseOnDeath = .20f;
    public float PercentOfExpToLoseOnDeath { get { return percentOfExpToLoseOnDeath; } private set { percentOfExpToLoseOnDeath = value; } }

    [SerializeField] private float demonKingExpLossDeath = 0.4f;
    public float DemonKingExpLossDeath { get { return demonKingExpLossDeath; } private set { demonKingExpLossDeath = value; } }


    [Header("EVOLUTION TYPES")]
    [SerializeField] private ExperienceBranch green;
    [SerializeField] private ExperienceBranch red;
    [SerializeField] private ExperienceBranch blue;

    [Header("MINION TYPES")]
    [SerializeField] private MinionType redMinion;
    [SerializeField] private MinionType greenMinion;
    [SerializeField] private MinionType blueMinion;

    private EvolutionManager evolutionManager;
    private DemonKingEvolution demonKingEvolution;

    private Vector3 BaseScale;
    private float baseCamDist;
    private float baseCamShoulderX;
    private float baseCamShoulderY;


    [HideInInspector] public ExperienceBranch CurrentActiveEvolutionBranch;

    private List<MinionType> minionTypes = new List<MinionType>();

    private Cinemachine3rdPersonFollow vCam;

    private PlayerHealthManager healthManager;

    #region Start Up
    private void Awake()
    {
        healthManager = GetComponent<PlayerHealthManager>();

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
            baseCamShoulderY = vCam.ShoulderOffset.y;


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
    #endregion

    #region ExperienceManagerSetup

    private void SetMyMinionTypeOnStart()
    {
        //Get a random location
        int randomMinionType = Random.Range(0, minionTypes.Count);
        SetMyMionionType(randomMinionType);
    }

    void SetMyMionionType(int minionType)
    {
        photonView.RPC("SetMinionType_RPC", RpcTarget.All, minionType);
    }

    [PunRPC]
    void SetMinionType_RPC(int minionType)
    {
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


    #endregion

    #region ExperienceBranchFunctions

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
    //Update the correct branch based off the given parameters
    void UpdateBranchType(ExperienceBranch branchType, int value)
    {
        branchType.ExpBar.CurrentExp = Mathf.Clamp(branchType.ExpBar.CurrentExp + value, 0, branchType.ExpBar.level2ExpNeeded.value);
        branchType.ExpBar.UpdateExpSlider();

        UpdateActiveBranchUI(branchType);

        if (branchType == CurrentActiveEvolutionBranch && !demonKingEvolution.AmITheDemonKing)
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

    public void DecreaseExperince(float decreaseValue)
    {
        UpdateExpBarOnDecrease(red, decreaseValue);
        UpdateExpBarOnDecrease(green, decreaseValue);
        UpdateExpBarOnDecrease(blue, decreaseValue);
    }
    void UpdateExpBarOnDecrease(ExperienceBranch branchToUpdate, float decreaseValue)
    {
        branchToUpdate.ExpBar.CurrentExp = Mathf.Clamp(branchToUpdate.ExpBar.CurrentExp - (branchToUpdate.ExpBar.CurrentExp * decreaseValue), 0, branchToUpdate.ExpBar.level2ExpNeeded.value);
        branchToUpdate.ExpBar.UpdateExpSlider();
    }
    #endregion

    #region EvolutionFunctions
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

    public void ScaleSize(float CurrentExp)
    {
        transform.localScale = BaseScale + Vector3.one * CurrentExp * ScaleAmount;
        if (CurrentExp >= 1)
        {
            vCam.CameraDistance = baseCamDist + CurrentExp * CamDistanceIncreaseAmount;
            vCam.ShoulderOffset.x = baseCamShoulderX + CurrentExp * CamShoulderOffsetXIncreaseAmount;
            vCam.ShoulderOffset.y = baseCamShoulderY + CurrentExp * CamShoulderOffsetYIncreaseAmount;

        }
        else
        {
            vCam.CameraDistance = baseCamDist;
            vCam.ShoulderOffset.x = baseCamShoulderX;
            vCam.ShoulderOffset.y = baseCamShoulderY;
        }
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
        healthManager.invulnerable = true;
        evolutionManager.nextEvolution = CurrentActiveEvolutionBranch.DemonKingEvolution;
        evolutionManager.ChangeEvolution(evolutionManager.nextEvolution, true);
    }
    #endregion
}
