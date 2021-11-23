using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using System.Collections;

public class ExperienceManager : MonoBehaviourPun
{
    [Header("Modifible stats")]
    [SerializeField] private float ScaleAmount = 0.1f;
    [SerializeField] private float CamDistanceIncreaseAmount = .5f;
    [SerializeField] private float CamShoulderOffsetXIncreaseAmount = 0f;
    [SerializeField] private float CamShoulderOffsetYIncreaseAmount = 0f;
    [SerializeField] private float ScaleLerpSpeed = 1f;
    [SerializeField] private float MaxGroundMoveSpeedScaleAmount = 1f;
    [SerializeField] private float MaxInAirMoveSpeedScaleAmount = 1f;


    [SerializeField] private float percentOfExpToLoseOnDeath = .20f;
    public float PercentOfExpToLoseOnDeath { get { return percentOfExpToLoseOnDeath; } private set { percentOfExpToLoseOnDeath = value; } }

    [SerializeField] private float demonKingExpLossDeath = 0.4f;
    public float DemonKingExpLossDeath { get { return demonKingExpLossDeath; } private set { demonKingExpLossDeath = value; } }

    public List<ExperienceBranch> ExperienceBranches;
    private EvolutionManager evolutionManager;
    private DemonKingEvolution demonKingEvolution;

    private Vector3 BaseScale;
    private float baseCamDist;
    private float baseCamShoulderX;
    private float baseCamShoulderY;

    private float baseMaxGroundSpeed;
    private float baseMaxInAirSpeed;
    

    public ExperienceBranch CurrentActiveEvolutionTypeBranch;

    private Cinemachine3rdPersonFollow vCam;

    private PlayerController playerController;

    Vector3 valueToLerpTowards;
    bool shouldScale;


    #region Start Up
    private void Awake()
    {
        if (photonView.IsMine)
        {
            //Get Required Components
            playerController = GetComponent<PlayerController>();
            vCam = gameObject.GetComponentInChildren<Cinemachine3rdPersonFollow>();
            evolutionManager = GetComponent<EvolutionManager>();
            demonKingEvolution = GetComponent<DemonKingEvolution>();

            //Get starting base values of scaleable variables
            BaseScale = transform.localScale;
            baseCamDist = vCam.CameraDistance;
            baseCamShoulderX = vCam.ShoulderOffset.x;
            baseCamShoulderY = vCam.ShoulderOffset.y;
            baseMaxGroundSpeed = playerController.MaxGroundMoveSpeed;
            baseMaxInAirSpeed = playerController.MaxAirMoveSpeed;

            SetExpSliders();
            foreach (var experienceBranch in ExperienceBranches)
            {
                experienceBranch.ExpBar.UpdateActiveExpBarCanEvolveText();
            }
        }
    }
    #endregion

    private void Update()
    {
        if (shouldScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, valueToLerpTowards, ScaleLerpSpeed * Time.deltaTime);
            if (transform.localScale == valueToLerpTowards)
                shouldScale = false;
        }
    }

    #region ExperienceManagerSetup
    //Set sliders max and current values called locally in Awake()
    void SetExpSliders()
    {
        foreach (var experienceBranch in ExperienceBranches)
        {
            experienceBranch.ExpBar.expMaterialCopy = Instantiate(experienceBranch.ExpBar.expMaterial);
            experienceBranch.ExpBar.fillImage.material = experienceBranch.ExpBar.expMaterialCopy;
        }
    }
    #endregion

    #region ExperienceBranchFunctions

    void UpdateBranchType(ExperienceBranch branchType, float value)
    {
        float prevExp = branchType.ExpBar.CurrentExp;

        //Update current exp value and sliders
        branchType.ExpBar.CurrentExp = Mathf.Clamp(branchType.ExpBar.CurrentExp + value, 0, branchType.ExpBar.level2ExpNeeded.value);
        StartCoroutine(branchType.ExpBar.UpdateExpBar(branchType.ExpBar.CurrentExp, prevExp));
        //Set UI to show active branch type

        //Scale size of player if eating same type as our evolution
        if (branchType == CurrentActiveEvolutionTypeBranch && !demonKingEvolution.AmITheDemonKing)
            ScaleSize(branchType.ExpBar.CurrentExp);

        SetCanEvolveFalse();


        // if experience is greater than level 2
        if (branchType.ExpBar.CurrentExp >= branchType.ExpBar.level2ExpNeeded.value)
        {
            if (evolutionManager.activeEvolution != branchType.Level2Evolution)
            {
                //Update next evolution to exp that just hit threshold
                evolutionManager.nextEvolution = branchType.Level2Evolution;
                SetCanEvolveTrue(branchType);
            }
        }
        // if experience is greater than level 1
        else if (branchType.ExpBar.CurrentExp >= branchType.ExpBar.level1ExpNeeded.value)
        {
            if (evolutionManager.activeEvolution != branchType.Level1Evolution)
            {
                //Update next evolution to exp that just hit threshold
                evolutionManager.nextEvolution = branchType.Level1Evolution;
                SetCanEvolveTrue(branchType);
                branchType.ExpBar.expThreshholdBar.SetActive(true);
            }
        }

        StartCoroutine(UpdateBranchUI(branchType));
    }

    //keep track of what branch the current evolution is (Only changed when evolve)
    public void UpdateCurrentActiveEvolutionTypeBranch(MinionType minionType, bool startValues)
    {
        foreach (var experienceBranch in ExperienceBranches)
        {
            if (minionType == experienceBranch.branchMinionType)
            {
                CurrentActiveEvolutionTypeBranch = experienceBranch;
                if (!startValues)
                    StartCoroutine(UpdateBranchUI(experienceBranch));
                break;
            }
        }
    }

    IEnumerator UpdateBranchUI(ExperienceBranch branchType)
    {
        foreach (var experienceBranch in ExperienceBranches)
        {
            if (branchType == experienceBranch)
            {
                experienceBranch.ExpBar.ActiveExpBarBackground.SetActive(true);
                        
                if (experienceBranch.CanEvolve && !demonKingEvolution.AmITheDemonKing)
                {
                    experienceBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(true);
                    experienceBranch.ExpBar.displayImg.SetActive(false);
                }
                
                float lerpTime = 0;

                while (lerpTime < 2)
                {
                    experienceBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(experienceBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1.1f, 1.2f, 1), 5 * Time.deltaTime);
                    lerpTime += Time.deltaTime;
                }
            }
            else
            {
                experienceBranch.ExpBar.displayImg.SetActive(true);
                experienceBranch.ExpBar.ActiveExpBarBackground.SetActive(false);
                experienceBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(false);
                
                float lerpTime = 0;
                while (lerpTime < 2)
                {
                    experienceBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(experienceBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1f, 1f, 1f), 5 * Time.deltaTime);
                    lerpTime += Time.deltaTime;
                }
            }

        }
        
        yield return null;
    }


    //Add the experience based of minion type eaten (called when killed minion or player)
    public void AddExpereince(MinionType minionType, float expValue)
    {
        foreach (var experienceBranch in ExperienceBranches)
        {
            if (minionType == experienceBranch.branchMinionType)
                UpdateBranchType(experienceBranch, expValue);
        }
      
    }

    //Decreases all exp values
    public void DecreaseExperince(float decreaseValue)
    {
        SetCanEvolveFalse();
        StartCoroutine(UpdateBranchUI(CurrentActiveEvolutionTypeBranch));
        
        foreach (var experienceBranch in ExperienceBranches)
        {
            UpdateExpBarOnDecrease(experienceBranch, decreaseValue);
        }
    }

    //Decreases all exp my given % and updates exp sliders
    void UpdateExpBarOnDecrease(ExperienceBranch branchToUpdate, float decreaseValue)
    {
        float prevExp = branchToUpdate.ExpBar.CurrentExp;
        branchToUpdate.ExpBar.CurrentExp = Mathf.Clamp(branchToUpdate.ExpBar.CurrentExp - (branchToUpdate.ExpBar.CurrentExp * decreaseValue), 0, branchToUpdate.ExpBar.level2ExpNeeded.value);
        StartCoroutine(branchToUpdate.ExpBar.UpdateExpBar(branchToUpdate.ExpBar.CurrentExp, prevExp));

        if (branchToUpdate.ExpBar.CurrentExp < branchToUpdate.ExpBar.level1ExpNeeded.value)
            branchToUpdate.ExpBar.expThreshholdBar.SetActive(false);
    }
    #endregion

    #region EvolutionFunctions
    //Sets can evolve based of branch type passed in
    public void SetCanEvolveTrue(ExperienceBranch branch)
    {
        foreach (var experienceBranch in ExperienceBranches)
        {
            if (branch == experienceBranch)
                experienceBranch.CanEvolve = true;
            else
            {
                experienceBranch.CanEvolve = false;
                experienceBranch.CanEvolve = false;
            }
        }
    }
    //This runs inside the evolution Manager when the evolution button has been pressed
    public bool CanEvolve()
    {
        foreach (var experienceBranch in ExperienceBranches)
        {
            if (experienceBranch.CanEvolve)
            {
                return true;
            }
        }
        return false;
    }


    public void ScaleSize(float CurrentExp)
    {
        valueToLerpTowards = BaseScale + Vector3.one * CurrentExp * ScaleAmount;
        shouldScale = true;

        //transform.localScale = BaseScale + Vector3.one * CurrentExp * ScaleAmount;
        if (CurrentExp > 0)
        {
            vCam.CameraDistance = baseCamDist + CurrentExp * CamDistanceIncreaseAmount;
            vCam.ShoulderOffset.x = baseCamShoulderX + CurrentExp * CamShoulderOffsetXIncreaseAmount;
            vCam.ShoulderOffset.y = baseCamShoulderY + CurrentExp * CamShoulderOffsetYIncreaseAmount;
            playerController.MaxGroundMoveSpeed = baseMaxGroundSpeed + CurrentExp * MaxGroundMoveSpeedScaleAmount;
            playerController.MaxAirMoveSpeed = baseMaxInAirSpeed + CurrentExp * MaxInAirMoveSpeedScaleAmount;
        }
        else
        {
            vCam.CameraDistance = baseCamDist;
            vCam.ShoulderOffset.x = baseCamShoulderX;
            vCam.ShoulderOffset.y = baseCamShoulderY;
            playerController.MaxGroundMoveSpeed = baseMaxGroundSpeed;
            playerController.MaxAirMoveSpeed = baseMaxInAirSpeed;
        }
    }

    //checks if players current branch dropped below exp threshold (Called after player respawns)
    public void CheckIfNeedToDevolve()
    {
        //Get ref to exp bar
        ExperienceBar currentExpBar = CurrentActiveEvolutionTypeBranch.ExpBar;

        //Scale size down to current exp value
        ScaleSize(currentExpBar.CurrentExp);

        //If exp is less then level 1 && and if we arent already lvl 0
        if (currentExpBar.CurrentExp < currentExpBar.level1ExpNeeded.value && evolutionManager.activeEvolution != CurrentActiveEvolutionTypeBranch.Level0Evolution)
        {
            evolutionManager.nextEvolution = CurrentActiveEvolutionTypeBranch.Level0Evolution;
            evolutionManager.SwapEvolution(evolutionManager.nextEvolution);
        }
        //If exp is greater then lvl 1 and if we arent already lvl 1
        else if (currentExpBar.CurrentExp >= currentExpBar.level1ExpNeeded.value && evolutionManager.activeEvolution != CurrentActiveEvolutionTypeBranch.Level1Evolution)
        {
            evolutionManager.nextEvolution = CurrentActiveEvolutionTypeBranch.Level1Evolution;
            evolutionManager.SwapEvolution(evolutionManager.nextEvolution);
        }

        MinionType currentHighestMinionType = null;
        float highestBranchExp = 0;

        foreach (var experienceBranch in ExperienceBranches)
        {
            if (experienceBranch.ExpBar.CurrentExp > highestBranchExp)
            {
                highestBranchExp = experienceBranch.ExpBar.CurrentExp;
                currentHighestMinionType = experienceBranch.branchMinionType;
            }
        }

        if (currentHighestMinionType != null)
            UpdateCurrentActiveEvolutionTypeBranch(currentHighestMinionType, false);

        if (CurrentActiveEvolutionTypeBranch.ExpBar.CurrentExp >= CurrentActiveEvolutionTypeBranch.ExpBar.level2ExpNeeded.value)
        {
            if (evolutionManager.activeEvolution != CurrentActiveEvolutionTypeBranch.Level2Evolution)
            {
                //Update next evolution to exp that just hit threshold
                evolutionManager.nextEvolution = CurrentActiveEvolutionTypeBranch.Level2Evolution;
                SetCanEvolveTrue(CurrentActiveEvolutionTypeBranch);
            }
        }
        // if experience is greater than level 1
        else if (CurrentActiveEvolutionTypeBranch.ExpBar.CurrentExp >= CurrentActiveEvolutionTypeBranch.ExpBar.level1ExpNeeded.value)
        {
            if (evolutionManager.activeEvolution != CurrentActiveEvolutionTypeBranch.Level1Evolution)
            {
                //Update next evolution to exp that just hit threshold
                evolutionManager.nextEvolution = CurrentActiveEvolutionTypeBranch.Level1Evolution;
                SetCanEvolveTrue(CurrentActiveEvolutionTypeBranch);
                CurrentActiveEvolutionTypeBranch.ExpBar.expThreshholdBar.SetActive(true);
            }
        }
        StartCoroutine(UpdateBranchUI(CurrentActiveEvolutionTypeBranch));
    }

    public void SetCanEvolveFalse()
    {
        //Player has evolved, can not evolve again
        foreach (var experienceBranch in ExperienceBranches)
        {
            experienceBranch.CanEvolve = false;
            experienceBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(false);
            experienceBranch.ExpBar.displayImg.SetActive(true);
        }
    }
    #endregion
}
