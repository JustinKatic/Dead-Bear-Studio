using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class EvolutionManager : MonoBehaviourPun
{
    [Header("Modifiable stats")]
    [SerializeField] private float TimeToEvolve = 3f;
    [SerializeField] private GameObject EvolveVFX;
    [SerializeField] private GameObject DemonKingEvolveVFX;

    //List to hold all evolutions
    [HideInInspector] public List<Evolutions> evolutions = new List<Evolutions>();
    [HideInInspector] public Evolutions activeEvolution;
    [HideInInspector] public Evolutions nextEvolution;
    [HideInInspector] public ExperienceBranch nextBranchType;

    //Components
    private PlayerController playerController;
    private ExperienceManager experienceManager;
    private PlayerHealthManager healthManager;
    private DemonKingEvolution demonKingEvolution;
    private PlayerTimers playerTimers;

    private bool evolving = false;

    private IEnumerator changeEvolutionCo;


    #region Start Up
    private void Awake()
    {
        //Run on all player objects
        //Gets list of all evolutions on this player
        evolutions = GetComponentsInChildren<Evolutions>(true).ToList();

        experienceManager = GetComponent<ExperienceManager>();
        healthManager = GetComponent<PlayerHealthManager>();
    }

    void Start()
    {
        //Loops through all evolutions on model and looks for active model and sets that as active evolution
        foreach (var evolution in evolutions)
        {
            if (evolution.gameObject.activeSelf)
            {
                activeEvolution = evolution;
                break;
            }
        }

        //Run local
        if (photonView.IsMine)
        {
            //get required components
            playerController = GetComponent<PlayerController>();
            demonKingEvolution = GetComponent<DemonKingEvolution>();
            playerTimers = GetComponentInChildren<PlayerTimers>();

            PlayerSoundManager.Instance.ChangeCurrentEvolutionSounds(activeEvolution.ModelAnimationSounds);

            playerController.currentAnim = activeEvolution.animator;
            playerController.CharacterInputs.Player.Evolve.performed += Evolve_performed;
        }
    }
    #endregion

    #region Update Loops
    private void Update()
    {
        if (photonView.IsMine)
        {
            if (evolving && healthManager.isStunned)
                InteruptEvolution();
        }
    }
    #endregion

    #region Evolve
    //called on evolve performed Input (Run locally)
    private void Evolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //If can evolve change evolution to my next evolution
        if (experienceManager.CanEvolve() && !evolving && !demonKingEvolution.AmITheDemonKing)
        {
            ChangeEvolution(nextEvolution, true);
        }
    }

    void Evolve(string currentModelsTag, string nextModelsTag)
    {
        photonView.RPC("Evolve_RPC", RpcTarget.All, currentModelsTag, nextModelsTag);
    }

    [PunRPC]
    public void Evolve_RPC(string currentModelsTag, string nextModelsTag)
    {
        foreach (var evolution in evolutions)
        {
            if (evolution.tag == currentModelsTag)
                evolution.gameObject.SetActive(false);
            if (evolution.tag == nextModelsTag)
            {
                evolution.gameObject.SetActive(true);
                healthManager.MyMinionType = evolution.MyMinionType;
            }
        }
    }

    public void ChangeEvolution(Evolutions evolution, bool ShouldPlayTransition)
    {
        changeEvolutionCo = ChangeEvolutionAfterX();
        StartCoroutine(changeEvolutionCo);

        IEnumerator ChangeEvolutionAfterX()
        {
            if (ShouldPlayTransition)
            {
                if (demonKingEvolution.AmITheDemonKing)
                {
                    PlayDemonKingEvolveVFX(true);
                    PlayerSoundManager.Instance.PlayDemonKingEvolveSound();
                }
                else
                {
                    PlayEvolveVFX(true);
                    PlayerSoundManager.Instance.PlayEvolveSound();
                }

                playerTimers.StartEvolveTimer(TimeToEvolve);
                playerController.DisableMovement();
            }
            else
            {
                SwapEvolution(evolution);
            }
            evolving = true;

            yield return new WaitForSeconds(TimeToEvolve);

            healthManager.invulnerable = false;
            evolving = false;
            playerTimers.StopEvolveTimer();
            if (demonKingEvolution.AmITheDemonKing)
            {
                PlayDemonKingEvolveVFX(false);
                PlayerSoundManager.Instance.StopDemonKingEvolveSound();
                PlayerSoundManager.Instance.PlayDemonKingAnnouncementSound();
            }
            else
            {
                PlayEvolveVFX(false);
                PlayerSoundManager.Instance.StopEvolveSound();
            }

            playerController.EnableMovement();

            SwapEvolution(evolution);
        }
    }

    void InteruptEvolution()
    {
        StopCoroutine(changeEvolutionCo);
        playerTimers.StopEvolveTimer();
        PlayEvolveVFX(false);
        PlayerSoundManager.Instance.StopEvolveSound();
        evolving = false;
        experienceManager.ChangeEvolutionBools(nextBranchType);
    }

    void SwapEvolution(Evolutions evolution)
    {
        experienceManager.SetCanEvolveFalse();
        Evolve(activeEvolution.tag, nextEvolution.tag);
        activeEvolution = evolution;
        experienceManager.CurrentActiveEvolutionBranch = nextBranchType;

        if (demonKingEvolution.AmITheDemonKing)
            experienceManager.ScaleSize(demonKingEvolution.ScaleAmount);
        else
            experienceManager.ScaleSize(nextBranchType.ExpBar.CurrentExp);

        PlayerSoundManager.Instance.ChangeCurrentEvolutionSounds(activeEvolution.ModelAnimationSounds);
        playerController.currentAnim = activeEvolution.animator;
        healthManager.SetHealth(activeEvolution.MaxHealth);
    }
    #endregion

    #region Play VFX
    void PlayEvolveVFX(bool Enabled)
    {
        photonView.RPC("PlayEvolveVFX_RPC", RpcTarget.All, Enabled);
    }

    [PunRPC]
    void PlayEvolveVFX_RPC(bool enabled)
    {
        if (enabled)
            EvolveVFX.SetActive(true);
        else
            EvolveVFX.SetActive(false);
    }

    void PlayDemonKingEvolveVFX(bool Enabled)
    {
        photonView.RPC("DemonKingEvolutionVFX", RpcTarget.All, Enabled);
    }

    [PunRPC]
    void DemonKingEvolutionVFX(bool enabled)
    {
        if (enabled)
            DemonKingEvolveVFX.SetActive(true);
        else
            DemonKingEvolveVFX.SetActive(false);
    }
    #endregion
}
