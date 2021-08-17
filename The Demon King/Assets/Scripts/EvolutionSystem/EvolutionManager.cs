using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;


public class EvolutionManager : MonoBehaviourPun
{
    [Header("Modifiable stats")]
    public float TimeToEvolve = 3f;

    //List to hold all evolutions
    [HideInInspector] public List<Evolutions> evolutions = new List<Evolutions>();
    [HideInInspector] public Evolutions activeEvolution;
    [HideInInspector] public Evolutions nextEvolution;
    [HideInInspector] public ExperienceBranch nextBranchType;

    [HideInInspector] public Transform currentActiveShootPoint;

    //Components
    private PlayerController playerController;
    private ExperienceManager experienceManager;
    private PlayerHealthManager healthManager;
    private DemonKingEvolution demonKingEvolution;

    private bool evolving = false;

    public GameObject EvolveVFX;
    public GameObject DemonKingEvolveVFX;

    private IEnumerator changeEvolutionCo;

    private void Awake()
    {
        //Run on all player objects
        //Gets list of all evolutions on this player
        evolutions = GetComponentsInChildren<Evolutions>(true).ToList();

        //gets access to exp manager on this player
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

            //sets shootPoint and anim of this player to the activeEvolutions
            currentActiveShootPoint = activeEvolution.ShootPoint;
            PlayerSoundManager.Instance.ChangeCurrentEvolutionSounds(activeEvolution.ModelAnimationSounds);

            playerController.currentAnim = activeEvolution.animator;
            playerController.CharacterInputs.Player.Evolve.performed += Evolve_performed;
        }
    }

    private void Update()
    {
        if (evolving && healthManager.isStunned)
            InteruptEvolution();
    }

    //called on evolve performed Input (Run locally)
    private void Evolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //If can evolve change evolution to my next evolution
        if (experienceManager.CanEvolve() && !evolving && !demonKingEvolution.AmITheDemonKing)
        {
            ChangeEvolution(nextEvolution, true);
        }
    }


    [PunRPC]
    public void Evolve(string currentModelsTag, string nextModelsTag)
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

    [PunRPC]
    void EvolutionVFX(bool enabled)
    {
        if (enabled)
            EvolveVFX.SetActive(true);
        else
            EvolveVFX.SetActive(false);
    }

    [PunRPC]
    void DemonKingEvolutionVFX(bool enabled)
    {
        if (enabled)
            DemonKingEvolveVFX.SetActive(true);
        else
            DemonKingEvolveVFX.SetActive(false);
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
                    photonView.RPC("DemonKingEvolutionVFX", RpcTarget.All, true);
                    PlayerSoundManager.Instance.PlayDemonKingEvolveSound();
                }
                else
                {
                    photonView.RPC("EvolutionVFX", RpcTarget.All, true);
                    PlayerSoundManager.Instance.PlayEvolveSound();
                }

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
            if (demonKingEvolution.AmITheDemonKing)
            {
                photonView.RPC("DemonKingEvolutionVFX", RpcTarget.All, false);
                PlayerSoundManager.Instance.StopDemonKingEvolveSound();
            }
            else
            {
                photonView.RPC("EvolutionVFX", RpcTarget.All, false);
                PlayerSoundManager.Instance.StopEvolveSound();
            }

            playerController.EnableMovement();


            SwapEvolution(evolution);
        }
    }

    void SwapEvolution(Evolutions evolution)
    {
        experienceManager.SetCanEvolveFalse();
        photonView.RPC("Evolve", RpcTarget.All, activeEvolution.tag, nextEvolution.tag);
        activeEvolution = evolution;
        experienceManager.CurrentActiveEvolutionBranch = nextBranchType;

        if (demonKingEvolution.AmITheDemonKing)
            experienceManager.ScaleSize(demonKingEvolution.ScaleAmount);
        else
            experienceManager.ScaleSize(nextBranchType.ExpBar.CurrentExp);

        currentActiveShootPoint = activeEvolution.ShootPoint;
        PlayerSoundManager.Instance.ChangeCurrentEvolutionSounds(activeEvolution.ModelAnimationSounds);
        playerController.currentAnim = activeEvolution.animator;
        photonView.RPC("SetHealth", RpcTarget.All, activeEvolution.MaxHealth);
    }

    void InteruptEvolution()
    {
        StopCoroutine(changeEvolutionCo);
        photonView.RPC("EvolutionVFX", RpcTarget.All, false);
        PlayerSoundManager.Instance.StopEvolveSound();
        evolving = false;
        experienceManager.ChangeEvolutionBools(nextBranchType);
    }
}
