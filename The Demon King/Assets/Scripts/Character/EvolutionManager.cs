using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class EvolutionManager : MonoBehaviourPun
{
    private GameObject currentActiveModel;
    [HideInInspector] public Transform currentActiveShootPoint;

    ExperienceManager experienceManager;

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
        currentActiveModel = experienceManager.CurrentEvolution.Model;
    }

    private void Evolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (experienceManager.CanEvolve())
            Evolve(experienceManager.NextEvolution);
    }


    [PunRPC]
    public void Evolve(GameObject currentModel, GameObject newModel)
    {
        currentModel.SetActive(false);
        newModel.SetActive(true);
    }


    public void Evolve(Evolutions evolution)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("Evolve", RpcTarget.All, experienceManager.CurrentEvolution.Model, experienceManager.NextEvolution.Model);
            currentActiveModel = evolution.Model;
            currentActiveShootPoint = evolution.ShootPoint;
            playerController.currentAnim = evolution.animator;
            photonView.RPC("SetHealth", RpcTarget.All, evolution.MaxHealth);
        }
    }
}
