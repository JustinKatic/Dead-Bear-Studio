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

            currentActiveModel = OozeModel;
            playerController.currentAnim = OozeAnimator;
        }
    }

    [ContextMenu("EvolveToMiximo")]
    public void EvolveToMiximo()
    {
        playerController.currentAnim = MiximoAnimator;
        currentActiveModel.SetActive(false);
        MiximoModel.SetActive(true);
        currentActiveModel = MiximoModel;
    }

    [ContextMenu("EvolveToOoze")]
    public void EvolveToOoze()
    {
        playerController.currentAnim = OozeAnimator;
        currentActiveModel.SetActive(false);
        OozeModel.SetActive(true);
        currentActiveModel = OozeModel;
    }
}
