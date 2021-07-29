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
        if (photonView.IsMine)
        {
            experienceManager = GetComponent<ExperienceManager>();
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
            photonView.RPC("Evolve", RpcTarget.All);
    }


    [PunRPC]
    public void Evolve()
    {
        Evolve(experienceManager.NextEvolution);
    }


    public void Evolve(Evolutions evolution)
    {
        currentActiveModel.SetActive(false);
        evolution.Model.SetActive(true);
        currentActiveModel = evolution.Model;

        if (photonView.IsMine)
        {
            currentActiveShootPoint = evolution.ShootPoint;
            playerController.currentAnim = evolution.animator;
            photonView.RPC("SetHealth", RpcTarget.All, evolution.MaxHealth);
        }
    }
}
