using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviourPun
{
    [SerializeField] private Image healthBarCurrentDisplayImg;

    [Header("Modifiable stats")]
    [SerializeField] private float TimeToEvolve = 3f;
    [SerializeField] private GameObject EvolveVFX;
    [SerializeField] private GameObject DemonKingEvolveVFX;
    [SerializeField] private Animator EvolveUIAnim;



    [Header("MINION TYPES")]
    [SerializeField] private MinionType redMinion;
    [SerializeField] private MinionType greenMinion;
    [SerializeField] private MinionType blueMinion;

    //List to hold all evolutions
    [HideInInspector] public List<Evolutions> evolutions = new List<Evolutions>();
    public Evolutions activeEvolution;
    [HideInInspector] public Evolutions nextEvolution;

    //Components
    private PlayerController playerController;
    private ExperienceManager experienceManager;
    private PlayerHealthManager playerHealthManager;
    private DemonKingEvolution demonKingEvolution;
    private PlayerTimers playerTimers;


    private LeaderboardManager leaderboardManager;

    [HideInInspector] public bool evolving = false;
    private KnockBackPlayer knockBackPlayer;


    private IEnumerator changeEvolutionCo;
    private IEnumerator EvolutionEffectCo;


    private List<MinionType> minionTypes = new List<MinionType>();

    public float TimeAsLionKingTimer;
    public float TimeAsRayKingTimer;
    public float TimeAsDragonKingTimer;

    public float TimeAsSlimeTimer;
    public float TimeAsLionTimer;
    public float TimeAsRayTimer;
    public float TimeAsDragonTimer;


    #region Start Up Handles getting components and setting starting evolution
    private void Awake()
    {
        //Add minions to list so can grab a random one as our type at start
        minionTypes.Add(greenMinion);
        minionTypes.Add(redMinion);
        minionTypes.Add(blueMinion);


        //Gets components that everyone needs access too
        evolutions = GetComponentsInChildren<Evolutions>(true).ToList();

        foreach (var evolution in evolutions)
        {
            SkinnedMeshRenderer[] children;
            children = evolution.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            var newMat = Instantiate(evolution.myMatInstance);
            evolution.myMatInstance = newMat;

            foreach (SkinnedMeshRenderer rend in children)
            {
                if (rend.material.name.Contains("Crown"))
                    continue;
                var mats = new Material[rend.materials.Length];
                for (var j = 0; j < rend.materials.Length; j++)
                {
                    mats[j] = newMat;
                }
                rend.materials = mats;
            }
        }

        playerHealthManager = GetComponent<PlayerHealthManager>();

        if (photonView.IsMine)
        {
            //get required components
            experienceManager = GetComponent<ExperienceManager>();
            playerController = GetComponent<PlayerController>();
            demonKingEvolution = GetComponent<DemonKingEvolution>();
            playerTimers = GetComponentInChildren<PlayerTimers>();
            leaderboardManager = FindObjectOfType<LeaderboardManager>();
            knockBackPlayer = GetComponent<KnockBackPlayer>();




            //get and set a random minion type
            SetMyMinionTypeOnStart();
            //set my active branch to my lvl0 evolution
            SetStartingActiveEvolution();
            //Set player active evolution active to everyone
            EvolveIntoStartType(activeEvolution.tag);
        }
    }
    private void Start()
    {
        if (photonView.IsMine)
        {
            PlayerSoundManager.Instance.ChangeCurrentEvolutionSounds(activeEvolution.ModelAnimationSounds);
            playerController.currentAnim = activeEvolution.animator;
            playerHealthManager.SetPlayerValuesOnEvolve(activeEvolution.MaxHealth, activeEvolution.ExpWorth, activeEvolution.ScoreWorth, activeEvolution.TimeTakenToBeDevoured, activeEvolution.healthRegenAmount, activeEvolution.DemonKingScoreWorth, activeEvolution.TimeTakenToBeDesinegrated);
            playerHealthManager.AmountOfHealthAddedAfterStunned = activeEvolution.AmountToHealAfterStunned;
            playerController.CharacterInputs.Player.Evolve.performed += Evolve_performed;
        }
    }
    #endregion


    #region Set Starting Type
    private void SetMyMinionTypeOnStart()
    {
        //Get a random location
        int randomMinionType = Random.Range(0, minionTypes.Count);
        playerHealthManager.MyMinionType = minionTypes[randomMinionType];
    }


    void SetStartingActiveEvolution()
    {
        //set our active evolution to our matching minion type
        if (playerHealthManager.MyMinionType == redMinion)
            activeEvolution = experienceManager.redBranch.Level0Evolution;
        else if (playerHealthManager.MyMinionType == blueMinion)
            activeEvolution = experienceManager.blueBranch.Level0Evolution;
        else if (playerHealthManager.MyMinionType == greenMinion)
            activeEvolution = experienceManager.greenBranch.Level0Evolution;

        //Set experince managers current active evolution type to our active type
        experienceManager.UpdateCurrentActiveEvolutionTypeBranch(activeEvolution.MyMinionType);
        healthBarCurrentDisplayImg.sprite = activeEvolution.MyHealthBarDisplaySprite;
    }

    void EvolveIntoStartType(string modelToSetActive)
    {
        photonView.RPC("EvolveIntoStartType_RPC", RpcTarget.All, modelToSetActive);
    }

    [PunRPC]
    void EvolveIntoStartType_RPC(string modelToSetActive)
    {
        foreach (var evolution in evolutions)
        {
            if (evolution.tag == modelToSetActive)
            {
                evolution.gameObject.SetActive(true);
                playerHealthManager.MyMinionType = evolution.MyMinionType;
                activeEvolution = evolution;
                return;
            }
        }
    }
    #endregion

    #region Update Loop handles interupting of evolution call
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (evolving && playerHealthManager.isStunned)
                InteruptEvolution();


            //Update time as a slime
            if (activeEvolution.gameObject.name.Contains("Ooze"))
                TimeAsSlimeTimer += Time.deltaTime;

            //Update time as Lion/Lion King
            else if (activeEvolution.MyMinionType == redMinion)
            {
                if (demonKingEvolution.AmITheDemonKing)
                    TimeAsLionKingTimer += Time.deltaTime;

                TimeAsLionTimer += Time.deltaTime;
            }

            //Update time as Dragon/Dragon King
            else if (activeEvolution.MyMinionType == greenMinion)
            {
                if (demonKingEvolution.AmITheDemonKing)
                    TimeAsLionKingTimer += Time.deltaTime;

                TimeAsDragonTimer += Time.deltaTime;
            }

            //Update time as Ray/Ray King
            else if (activeEvolution.MyMinionType == blueMinion)
            {
                if (demonKingEvolution.AmITheDemonKing)
                    TimeAsLionKingTimer += Time.deltaTime;

                TimeAsRayTimer += Time.deltaTime;
            }
        }
    }
    #endregion

    #region Handles evolving
    private void Evolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //If can evolve change evolution to my next evolution
        if (experienceManager.CanEvolve() && !evolving && !demonKingEvolution.AmITheDemonKing)
        {
            ChangeEvolution(nextEvolution);
        }
    }


    public void ChangeEvolution(Evolutions evolution)
    {
        changeEvolutionCo = ChangeEvolutionAfterX();
        StartCoroutine(changeEvolutionCo);

        IEnumerator ChangeEvolutionAfterX()
        {
            //Play evolving vfx depending chnages depending on evolving to king or not
            if (demonKingEvolution.AmITheDemonKing)
            {
                PlayDemonKingEvolveVFX(true);
                PlayerSoundManager.Instance.PlayDemonKingEvolveSound();
                playerHealthManager.invulnerable = true;
            }
            else
            {
                PlayEvolveVFX(true);
                PlayerSoundManager.Instance.PlayEvolveSound();
            }

            playerController.currentAnim.SetBool("Evolve", true);
            EvolveUIAnim.SetBool("Evolve", true);

            //Start UI Timer for evolving
            playerTimers.StartEvolveTimer(TimeToEvolve);
            //disable the players movement
            playerController.DisableMovement();

            //set evolving to true so update loop runs to check for interuption
            evolving = true;

            yield return new WaitForSeconds(TimeToEvolve);

            //Stop checking for interuption in update
            evolving = false;
            playerController.currentAnim.SetBool("Evolve", false);
            EvolveUIAnim.SetBool("Evolve", false);

            //Stop UI timer for evolving
            playerTimers.StopEvolveTimer();

            if (demonKingEvolution.AmITheDemonKing)
            {
                PlayDemonKingEvolveVFX(false);
                knockBackPlayer.SpawnPushBack();
                playerHealthManager.invulnerable = false;
                PlayerSoundManager.Instance.StopDemonKingEvolveSound();
                PlayerSoundManager.Instance.PlayDemonKingAnnouncementSound();
            }
            else
            {
                PlayEvolveVFX(false);
                PlayerSoundManager.Instance.StopEvolveSound();
            }
            //Enable player movement
            playerController.EnableMovement();

            //Swap player to new evolution model
            SwapEvolution(evolution);
        }
    }

    public void SwapEvolution(Evolutions evolution)
    {
        //Set can evolve to false so cant evolve multiple times to same thing
        experienceManager.SetCanEvolveFalse();
        //Change our model and minion type for everyone
        Evolve(activeEvolution.tag, nextEvolution.tag);
        //Update current active evolution
        activeEvolution = evolution;

        experienceManager.UpdateCurrentActiveEvolutionTypeBranch(evolution.MyMinionType);

        if (experienceManager.CurrentActiveEvolutionTypeBranch.ExpBar.CurrentExp > experienceManager.CurrentActiveEvolutionTypeBranch.ExpBar.level1ExpNeeded.value)
        {
            experienceManager.CurrentActiveEvolutionTypeBranch.ExpBar.adultDisplayImg.SetActive(true);
            experienceManager.CurrentActiveEvolutionTypeBranch.ExpBar.childDisplayImg.SetActive(false);
        }
        else
        {
            experienceManager.CurrentActiveEvolutionTypeBranch.ExpBar.adultDisplayImg.SetActive(false);
            experienceManager.CurrentActiveEvolutionTypeBranch.ExpBar.childDisplayImg.SetActive(true);
        }

        healthBarCurrentDisplayImg.sprite = activeEvolution.MyHealthBarDisplaySprite;

        //scale player to correct size
        if (demonKingEvolution.AmITheDemonKing)
            experienceManager.ScaleSize(demonKingEvolution.ScaleAmount);
        else
            experienceManager.ScaleSize(experienceManager.CurrentActiveEvolutionTypeBranch.ExpBar.CurrentExp);

        //Set players sounds animations and health to new evolutions
        PlayerSoundManager.Instance.ChangeCurrentEvolutionSounds(activeEvolution.ModelAnimationSounds);
        playerController.currentAnim = activeEvolution.animator;
        playerHealthManager.AmountOfHealthAddedAfterStunned = activeEvolution.AmountToHealAfterStunned;
        playerHealthManager.SetPlayerValuesOnEvolve(activeEvolution.MaxHealth, activeEvolution.ExpWorth, activeEvolution.ScoreWorth, activeEvolution.TimeTakenToBeDevoured, activeEvolution.healthRegenAmount, activeEvolution.DemonKingScoreWorth, activeEvolution.TimeTakenToBeDesinegrated);
        playerHealthManager.Heal(activeEvolution.AmountToHealWhenEvolveing);
        leaderboardManager.RaiseUpdateLeaderboardEvent();
    }

    void Evolve(string currentModelsTag, string nextModelsTag)
    {
        photonView.RPC("Evolve_RPC", RpcTarget.All, currentModelsTag, nextModelsTag);
    }

    [PunRPC]
    public void Evolve_RPC(string currentModelsTag, string nextModelsTag)
    {
        //Loop through all the evolutions on player and set the current model active and the model changing into active
        foreach (var evolution in evolutions)
        {
            if (evolution.tag == currentModelsTag)
                evolution.gameObject.SetActive(false);
            if (evolution.tag == nextModelsTag)
            {
                evolution.gameObject.SetActive(true);
                activeEvolution = evolution;
                playerHealthManager.MyMinionType = evolution.MyMinionType;
            }
        }
    }

    void InteruptEvolution()
    {
        StopCoroutine(changeEvolutionCo);
        playerTimers.StopEvolveTimer();
        PlayEvolveVFX(false);
        PlayerSoundManager.Instance.StopEvolveSound();
        playerController.currentAnim.SetBool("Evolve", false);
        EvolveUIAnim.SetBool("Evolve", false);
        evolving = false;
    }
    #endregion

    //This function is called by DemonKingEvolution script
    public void ActivateDemonKingEvolution()
    {
        nextEvolution = experienceManager.CurrentActiveEvolutionTypeBranch.DemonKingEvolution;
        ChangeEvolution(nextEvolution);
    }

    #region Play VFX
    void PlayEvolveVFX(bool Enabled)
    {
        photonView.RPC("PlayEvolveVFX_RPC", RpcTarget.All, Enabled);
    }

    [PunRPC]
    void PlayEvolveVFX_RPC(bool enabled)
    {
        if (enabled)
        {
            EvolveVFX.SetActive(true);

            EvolutionEffectCo = ToggleEvolveShader();
            StartCoroutine(EvolutionEffectCo);
        }
        else
        {
            EvolveVFX.SetActive(false);

            if (EvolutionEffectCo != null)
                StopCoroutine(EvolutionEffectCo);
            activeEvolution.myMatInstance.SetFloat("_EvolvingEffectTime", 0);
        }
    }

    IEnumerator ToggleEvolveShader()
    {
        float lerpTime = 0;

        while (lerpTime < TimeToEvolve)
        {
            float valToBeLerped = Mathf.Lerp(0, 1, (lerpTime / TimeToEvolve));
            lerpTime += Time.deltaTime;
            activeEvolution.myMatInstance.SetFloat("_EvolvingEffectTime", valToBeLerped);
            yield return null;
        }
        EvolutionEffectCo = null;
    }

    void PlayDemonKingEvolveVFX(bool Enabled)
    {
        photonView.RPC("DemonKingEvolutionVFX", RpcTarget.All, Enabled);
    }

    [PunRPC]
    void DemonKingEvolutionVFX(bool enabled)
    {
        if (enabled)
        {
            DemonKingEvolveVFX.SetActive(true);
            EvolutionEffectCo = ToggleEvolveShader();
            StartCoroutine(EvolutionEffectCo);
        }
        else
        {
            DemonKingEvolveVFX.SetActive(false);
            if (EvolutionEffectCo != null)
                StopCoroutine(EvolutionEffectCo);
            activeEvolution.myMatInstance.SetFloat("_EvolvingEffectTime", 0);
        }
    }
    #endregion
}
