using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EvolutionManager : MonoBehaviourPun
{
    [Header("Animations")]
    public Animator OozeAnimator;
    public Animator MiximoAnimator;

    [Header("Models")]
    public GameObject OozeModel;
    public GameObject MiximoModel;

    private GameObject currentActiveModel;


    PlayerController playerController;
    PlayerHealthManager playerHealth;

    void Awake()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            playerHealth = GetComponent<PlayerHealthManager>();

            playerController.currentAnim = OozeAnimator;

            playerController.CharacterInputs.Player.Evolve.performed += Evolve_performed;
            playerController.CharacterInputs.Player.Devolve.performed += Devolve_performed;
        }
        currentActiveModel = OozeModel;
    }

    private void Evolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        photonView.RPC("EvolveToMiximo", RpcTarget.All);
    }

    private void Devolve_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        photonView.RPC("EvolveToOoze", RpcTarget.All);
    }

    [PunRPC]
    public void EvolveToMiximo()
    {
        currentActiveModel.SetActive(false);
        MiximoModel.SetActive(true);
        currentActiveModel = MiximoModel;

        if (photonView.IsMine)
        {
            playerController.currentAnim = MiximoAnimator;
        }
    }

    [PunRPC]
    public void EvolveToOoze()
    {
        currentActiveModel.SetActive(false);
        OozeModel.SetActive(true);
        currentActiveModel = OozeModel;
        if (photonView.IsMine)
        {
            playerController.currentAnim = OozeAnimator;
        }
    }
}
