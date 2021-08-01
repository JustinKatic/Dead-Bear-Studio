using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;


public class EvolutionManager : MonoBehaviourPun
{
    [HideInInspector] public Transform currentActiveShootPoint;
    ExperienceManager experienceManager;

    [HideInInspector] public List<Evolutions> evolutions = new List<Evolutions>();

    PlayerController playerController;
    PlayerHealthManager playerHealth;
    private Evolutions activeEvolution;
    private Evolutions nextEvolution;
    
    void Start()
    {
        evolutions = GetComponentsInChildren<Evolutions>().ToList();
        experienceManager = GetComponent<ExperienceManager>();
        
        foreach (var evolution in evolutions)
        {
            if (evolution.gameObject.activeSelf)
            {
                activeEvolution = evolution;
                break;
            }
        }
        
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            playerHealth = GetComponent<PlayerHealthManager>();
            
            currentActiveShootPoint = activeEvolution.ShootPoint;
            playerController.currentAnim = activeEvolution.animator;
            playerController.CharacterInputs.Player.Evolve.performed += Evolve_performed;
        }

    }

    private void Evolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (experienceManager.CanEvolve())
        {
            ChangeEvolution(experienceManager.NextEvolution);
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
    
    public void ChangeEvolution(string evolution)
    {      
        photonView.RPC("Evolve", RpcTarget.All, experienceManager.CurrentEvolution, experienceManager.NextEvolution);
        experienceManager.CurrentEvolution = evolution;
        currentActiveShootPoint = activeEvolution.ShootPoint;
        playerController.currentAnim = activeEvolution.animator;
        photonView.RPC("SetHealth", RpcTarget.All, activeEvolution.MaxHealth);
    }
}
