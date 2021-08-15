using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CallWalkSoundEvent : MonoBehaviourPun
{
    PlayerSoundManager playerSoundManager;

    private void Start()
    {
        playerSoundManager = GetComponentInParent<PlayerSoundManager>();
    }

    private void FootStepSound()
    {
        if (photonView.IsMine)
            playerSoundManager.FootStepSound();
    }
}
