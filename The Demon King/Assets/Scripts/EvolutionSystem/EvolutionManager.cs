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

    public GameObject EvolveVFX;

    private IEnumerator changeEvolutionCo;

    private void Awake()
    {
        //Gets list of all evolutions on this player
        evolutions = GetComponentsInChildren<Evolutions>(true).ToList();

        //gets access to exp manager on this player
        experienceManager = GetComponent<ExperienceManager>();
    }

    void Start()
    {
        //Run on all player objects

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

            //sets shootPoint and anim of this player to the activeEvolutions
            currentActiveShootPoint = activeEvolution.ShootPoint;
            playerController.currentAnim = activeEvolution.animator;
            playerController.CharacterInputs.Player.Evolve.performed += Evolve_performed;
        }
    }

    //called on evolve performed Input (Run locally)
    private void Evolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //If can evolve change evolution to my next evolution
        if (experienceManager.CanEvolve())
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
                evolution.gameObject.SetActive(true);
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

    public void ChangeEvolution(Evolutions evolution, bool ShouldPlayTransition)
    {
        if (ShouldPlayTransition)
        {
            photonView.RPC("EvolutionVFX", RpcTarget.All, true);
            playerController.DisableMovement();
            changeEvolutionCo = ChangeEvolutionAfterX();
            StartCoroutine(changeEvolutionCo);
        }
        else
        {
            SwapEvolution(evolution);
        }

        IEnumerator ChangeEvolutionAfterX()
        {
            yield return new WaitForSeconds(TimeToEvolve);
            photonView.RPC("EvolutionVFX", RpcTarget.All, false);
            playerController.EnableMovement();

            SwapEvolution(evolution);
        }
    }


    void SwapEvolution(Evolutions evolution)
    {
        photonView.RPC("Evolve", RpcTarget.All, activeEvolution.tag, nextEvolution.tag);
        activeEvolution = evolution;
        experienceManager.currentBranch = nextBranchType;
        experienceManager.ScaleSizeUp(nextBranchType.ExpBar.CurrentExp);
        currentActiveShootPoint = activeEvolution.ShootPoint;
        playerController.currentAnim = activeEvolution.animator;
        photonView.RPC("SetHealth", RpcTarget.All, activeEvolution.MaxHealth);
    }
}
