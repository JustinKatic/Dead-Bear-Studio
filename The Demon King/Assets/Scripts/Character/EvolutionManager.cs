using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class EvolutionManager : MonoBehaviourPun
{
    [HideInInspector] public Transform currentActiveShootPoint;

    ExperienceManager experienceManager;

    public List<GameObject> models = new List<GameObject>();

    PlayerController playerController;
    PlayerHealthManager playerHealth;

    void Awake()
    {
        experienceManager = GetComponent<ExperienceManager>();
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            playerHealth = GetComponent<PlayerHealthManager>();

            currentActiveShootPoint = experienceManager.CurrentEvolution.ShootPoint;
            playerController.currentAnim = experienceManager.CurrentEvolution.animator;
            playerController.CharacterInputs.Player.Evolve.performed += Evolve_performed;
        }
    }

    private void Evolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (experienceManager.CanEvolve())
        {
            Evolve(experienceManager.NextEvolution);
        }
    }


    [PunRPC]
    public void Evolve(string currentModelsTag, string nextModelsTag)
    {
        foreach (var model in models)
        {
            if (model.tag == currentModelsTag)
                model.SetActive(false);
            if (model.tag == nextModelsTag)
                model.SetActive(true);
        }
    }


    public void Evolve(Evolutions evolution)
    {      
        photonView.RPC("Evolve", RpcTarget.All, experienceManager.CurrentEvolution.Model.tag, experienceManager.NextEvolution.Model.tag);
        experienceManager.CurrentEvolution = evolution;
        currentActiveShootPoint = evolution.ShootPoint;
        playerController.currentAnim = evolution.animator;
        photonView.RPC("SetHealth", RpcTarget.All, evolution.MaxHealth);
    }
}
