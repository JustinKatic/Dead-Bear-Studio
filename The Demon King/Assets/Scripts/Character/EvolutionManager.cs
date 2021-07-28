using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EvolutionManager : MonoBehaviourPun
{
    public Animator OozeAnimator;
    public Animator MiximoAnimator;

    PlayerController playerController;

    void Awake()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();

            if (playerController.currentAnim == null)
                playerController.currentAnim = OozeAnimator;
        }
    }

    public void EvolveToMiximo()
    {
        playerController.currentAnim = MiximoAnimator;
    }

    public void EvolveToOoze()
    {
        playerController.currentAnim = OozeAnimator;
    }





}
