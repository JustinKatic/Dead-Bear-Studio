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


    [Header("EVOLUTION TYPES")]
    public ExperienceBranch greenBranch;
    public ExperienceBranch redBranch;
    public ExperienceBranch blueBranch;

    private EvolutionManager evolutionManager;
    private DemonKingEvolution demonKingEvolution;

    private Vector3 BaseScale;
    private float baseCamDist;
    private float baseCamShoulderX;
    private float baseCamShoulderY;

    private float baseMaxGroundSpeed;
    private float baseMaxInAirSpeed;

    [Header("MINION TYPES")]
    [SerializeField] private MinionType redMinion;
    [SerializeField] private MinionType greenMinion;
    [SerializeField] private MinionType blueMinion;

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

            redBranch.ExpBar.UpdateActiveExpBarCanEvolveText();
            greenBranch.ExpBar.UpdateActiveExpBarCanEvolveText();
            blueBranch.ExpBar.UpdateActiveExpBarCanEvolveText();
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
        redBranch.ExpBar.expMaterialCopy = Instantiate(redBranch.ExpBar.expMaterial);
        redBranch.ExpBar.fillImage.material = redBranch.ExpBar.expMaterialCopy;

        greenBranch.ExpBar.expMaterialCopy = Instantiate(greenBranch.ExpBar.expMaterial);
        greenBranch.ExpBar.fillImage.material = greenBranch.ExpBar.expMaterialCopy;

        blueBranch.ExpBar.expMaterialCopy = Instantiate(blueBranch.ExpBar.expMaterial);
        blueBranch.ExpBar.fillImage.material = blueBranch.ExpBar.expMaterialCopy;
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
        if (minionType == redMinion)
        {
            CurrentActiveEvolutionTypeBranch = redBranch;
            if (!startValues)
                StartCoroutine(UpdateBranchUI(redBranch));
        }
        else if (minionType == greenMinion)
        {
            CurrentActiveEvolutionTypeBranch = greenBranch;
            if (!startValues)
                StartCoroutine(UpdateBranchUI(greenBranch));

        }
        else if (minionType == blueMinion)
        {
            CurrentActiveEvolutionTypeBranch = blueBranch;
            if (!startValues)
                StartCoroutine(UpdateBranchUI(blueBranch));
        }
    }



    IEnumerator UpdateBranchUI(ExperienceBranch branchType)
    {
        if (branchType == redBranch)
        {
            redBranch.ExpBar.ActiveExpBarBackground.SetActive(true);
            greenBranch.ExpBar.ActiveExpBarBackground.SetActive(false);
            blueBranch.ExpBar.ActiveExpBarBackground.SetActive(false);

            if (redBranch.CanEvolve && !demonKingEvolution.AmITheDemonKing)

            {
                redBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(true);
                redBranch.ExpBar.adultDisplayImg.SetActive(false);
                redBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else
            {
                redBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(false);
                if (redBranch.ExpBar.CurrentExp >= redBranch.ExpBar.level1ExpNeeded.value)
                {
                    redBranch.ExpBar.adultDisplayImg.SetActive(true);
                    redBranch.ExpBar.childDisplayImg.SetActive(false);
                }
                else if (redBranch.ExpBar.CurrentExp >= redBranch.ExpBar.level2ExpNeeded.value)
                {
                    redBranch.ExpBar.adultDisplayImg.SetActive(false);
                    redBranch.ExpBar.childDisplayImg.SetActive(true);
                }
            }

            if (greenBranch.ExpBar.CurrentExp >= greenBranch.ExpBar.level1ExpNeeded.value)
            {
                greenBranch.ExpBar.adultDisplayImg.SetActive(true);
                greenBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else if (greenBranch.ExpBar.CurrentExp >= greenBranch.ExpBar.level2ExpNeeded.value)
            {
                greenBranch.ExpBar.adultDisplayImg.SetActive(false);
                greenBranch.ExpBar.childDisplayImg.SetActive(true);
            }

            if (blueBranch.ExpBar.CurrentExp >= blueBranch.ExpBar.level1ExpNeeded.value)
            {
                blueBranch.ExpBar.adultDisplayImg.SetActive(true);
                blueBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else if (blueBranch.ExpBar.CurrentExp >= blueBranch.ExpBar.level2ExpNeeded.value)
            {
                blueBranch.ExpBar.adultDisplayImg.SetActive(false);
                blueBranch.ExpBar.childDisplayImg.SetActive(true);
            }


            float lerpTime = 0;

            while (lerpTime < 2)
            {
                redBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(redBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1.1f, 1.2f, 1), 5 * Time.deltaTime);
                greenBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(greenBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1f, 1f, 1), 5 * Time.deltaTime);
                blueBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(blueBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1f, 1f, 1), 5 * Time.deltaTime);
                lerpTime += Time.deltaTime;

                yield return null;
            }
        }
        else if (branchType == greenBranch && !demonKingEvolution.AmITheDemonKing)
        {
            redBranch.ExpBar.ActiveExpBarBackground.SetActive(false);
            greenBranch.ExpBar.ActiveExpBarBackground.SetActive(true);
            blueBranch.ExpBar.ActiveExpBarBackground.SetActive(false);

            if (greenBranch.CanEvolve)
            {
                greenBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(true);
                greenBranch.ExpBar.adultDisplayImg.SetActive(false);
                greenBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else
            {
                greenBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(false);
                if (greenBranch.ExpBar.CurrentExp >= greenBranch.ExpBar.level1ExpNeeded.value)
                {
                    greenBranch.ExpBar.adultDisplayImg.SetActive(true);
                    greenBranch.ExpBar.childDisplayImg.SetActive(false);
                }
                else if (greenBranch.ExpBar.CurrentExp >= greenBranch.ExpBar.level2ExpNeeded.value)
                {
                    greenBranch.ExpBar.adultDisplayImg.SetActive(false);
                    greenBranch.ExpBar.childDisplayImg.SetActive(true);
                }
            }

            if (redBranch.ExpBar.CurrentExp >= redBranch.ExpBar.level1ExpNeeded.value)
            {
                redBranch.ExpBar.adultDisplayImg.SetActive(true);
                redBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else if (redBranch.ExpBar.CurrentExp >= redBranch.ExpBar.level2ExpNeeded.value)
            {
                redBranch.ExpBar.adultDisplayImg.SetActive(false);
                redBranch.ExpBar.childDisplayImg.SetActive(true);
            }

            if (blueBranch.ExpBar.CurrentExp >= blueBranch.ExpBar.level1ExpNeeded.value)
            {
                blueBranch.ExpBar.adultDisplayImg.SetActive(true);
                blueBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else if (blueBranch.ExpBar.CurrentExp >= blueBranch.ExpBar.level2ExpNeeded.value)
            {
                blueBranch.ExpBar.adultDisplayImg.SetActive(false);
                blueBranch.ExpBar.childDisplayImg.SetActive(true);
            }


            float lerpTime = 0;

            while (lerpTime < 2)
            {
                redBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(redBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1f, 1f, 1), 5 * Time.deltaTime);
                greenBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(greenBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1.1f, 1.2f, 1), 5 * Time.deltaTime);
                blueBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(blueBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1f, 1f, 1), 5 * Time.deltaTime);
                lerpTime += Time.deltaTime;

                yield return null;
            }
        }
        else if (branchType == blueBranch && !demonKingEvolution.AmITheDemonKing)
        {
            redBranch.ExpBar.ActiveExpBarBackground.SetActive(false);
            greenBranch.ExpBar.ActiveExpBarBackground.SetActive(false);
            blueBranch.ExpBar.ActiveExpBarBackground.SetActive(true);

            if (blueBranch.CanEvolve)
            {
                blueBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(true);
                blueBranch.ExpBar.adultDisplayImg.SetActive(false);
                blueBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else
            {
                blueBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(false);
                if (blueBranch.ExpBar.CurrentExp >= blueBranch.ExpBar.level1ExpNeeded.value)
                {
                    blueBranch.ExpBar.adultDisplayImg.SetActive(true);
                    blueBranch.ExpBar.childDisplayImg.SetActive(false);
                }
                else if (blueBranch.ExpBar.CurrentExp >= blueBranch.ExpBar.level2ExpNeeded.value)
                {
                    blueBranch.ExpBar.adultDisplayImg.SetActive(false);
                    blueBranch.ExpBar.childDisplayImg.SetActive(true);
                }
            }

            if (greenBranch.ExpBar.CurrentExp >= greenBranch.ExpBar.level1ExpNeeded.value)
            {
                greenBranch.ExpBar.adultDisplayImg.SetActive(true);
                greenBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else if (greenBranch.ExpBar.CurrentExp >= greenBranch.ExpBar.level2ExpNeeded.value)
            {
                greenBranch.ExpBar.adultDisplayImg.SetActive(false);
                greenBranch.ExpBar.childDisplayImg.SetActive(true);
            }

            if (redBranch.ExpBar.CurrentExp >= redBranch.ExpBar.level1ExpNeeded.value)
            {
                redBranch.ExpBar.adultDisplayImg.SetActive(true);
                redBranch.ExpBar.childDisplayImg.SetActive(false);
            }
            else if (redBranch.ExpBar.CurrentExp >= redBranch.ExpBar.level2ExpNeeded.value)
            {
                redBranch.ExpBar.adultDisplayImg.SetActive(false);
                redBranch.ExpBar.childDisplayImg.SetActive(true);
            }

            float lerpTime = 0;

            while (lerpTime < 2)
            {
                redBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(redBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1f, 1f, 1), 5 * Time.deltaTime);
                greenBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(greenBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1f, 1f, 1), 5 * Time.deltaTime);
                blueBranch.ExpBar.expBarParent.transform.localScale = Vector3.Lerp(blueBranch.ExpBar.expBarParent.transform.localScale, new Vector3(1.1f, 1.2f, 1), 5 * Time.deltaTime);
                lerpTime += Time.deltaTime;

                yield return null;
            }
        }
    }


    //Add the experience based of minion type eaten (called when killed minion or player)
    public void AddExpereince(MinionType minionType, float expValue)
    {
        //update red exp
        if (minionType == redMinion)
            UpdateBranchType(redBranch, expValue);
        //update green exp
        else if (minionType == greenMinion)
            UpdateBranchType(greenBranch, expValue);
        //update blue exp
        else if (minionType == blueMinion)
            UpdateBranchType(blueBranch, expValue);
    }

    //Decreases all exp values
    public void DecreaseExperince(float decreaseValue)
    {
        SetCanEvolveFalse();
        StartCoroutine(UpdateBranchUI(CurrentActiveEvolutionTypeBranch));
        UpdateExpBarOnDecrease(redBranch, decreaseValue);
        UpdateExpBarOnDecrease(greenBranch, decreaseValue);
        UpdateExpBarOnDecrease(blueBranch, decreaseValue);
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
        if (branch == redBranch)
        {
            redBranch.CanEvolve = true;
            greenBranch.CanEvolve = false;
            blueBranch.CanEvolve = false;
        }
        else if (branch == greenBranch)
        {
            redBranch.CanEvolve = false;
            greenBranch.CanEvolve = true;
            blueBranch.CanEvolve = false;
        }
        else if (branch == blueBranch)
        {
            redBranch.CanEvolve = false;
            greenBranch.CanEvolve = false;
            blueBranch.CanEvolve = true;
        }

    }
    //This runs inside the evolution Manager when the evolution button has been pressed
    public bool CanEvolve()
    {
        //Check if I can evolve into any of these types is yes reset can evolve and return true else return false
        if (redBranch.CanEvolve || blueBranch.CanEvolve || greenBranch.CanEvolve)
        {
            return true;
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
    }

    public void SetCanEvolveFalse()
    {
        //Player has evolved, can not evolve again
        redBranch.CanEvolve = false;
        blueBranch.CanEvolve = false;
        greenBranch.CanEvolve = false;

        redBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(false);
        greenBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(false);
        blueBranch.ExpBar.ActiveExpBarCanEvolveTxt.SetActive(false);

        if (CurrentActiveEvolutionTypeBranch.ExpBar.CurrentExp > CurrentActiveEvolutionTypeBranch.ExpBar.level1ExpNeeded.value)
        {
            CurrentActiveEvolutionTypeBranch.ExpBar.adultDisplayImg.SetActive(true);
            CurrentActiveEvolutionTypeBranch.ExpBar.childDisplayImg.SetActive(false);
        }
        else
        {
            CurrentActiveEvolutionTypeBranch.ExpBar.adultDisplayImg.SetActive(false);
            CurrentActiveEvolutionTypeBranch.ExpBar.childDisplayImg.SetActive(true);
        }
    }
    #endregion
}
