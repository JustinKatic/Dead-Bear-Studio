using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public struct Evolution
{
    public GameObject Model;
    public Animator animator;
    public Transform ShootPoint;
    public int MaxHealth;
}


public class EvolutionManager : MonoBehaviourPun
{
    public Evolution Ooze;
    public Evolution Miximo;

    private GameObject currentActiveModel;
    [HideInInspector] public Transform currentActiveShootPoint;


    PlayerController playerController;
    PlayerHealthManager playerHealth;

    void Awake()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            playerHealth = GetComponent<PlayerHealthManager>();

            currentActiveShootPoint = Ooze.ShootPoint;
            playerController.currentAnim = Ooze.animator;

            playerController.CharacterInputs.Player.Evolve.performed += Evolve_performed;
            playerController.CharacterInputs.Player.Devolve.performed += Devolve_performed;
        }
        currentActiveModel = Ooze.Model;
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
        Miximo.Model.SetActive(true);
        currentActiveModel = Miximo.Model;

        if (photonView.IsMine)
        {
            currentActiveShootPoint = Miximo.ShootPoint;
            playerController.currentAnim = Miximo.animator;
            photonView.RPC("SetHealth", RpcTarget.All, Miximo.MaxHealth);
        }
    }

    [PunRPC]
    public void EvolveToOoze()
    {
        currentActiveModel.SetActive(false);
        Ooze.Model.SetActive(true);
        currentActiveModel = Ooze.Model;
        if (photonView.IsMine)
        {
            currentActiveShootPoint = Ooze.ShootPoint;
            playerController.currentAnim = Ooze.animator;
            photonView.RPC("SetHealth", RpcTarget.All, Ooze.MaxHealth);
        }
    }
}
