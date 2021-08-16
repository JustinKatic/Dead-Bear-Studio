using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CallWalkSoundEvent : MonoBehaviourPun
{
    private void FootStepSound()
    {
        if (photonView.IsMine)
            PlayerSoundManager.Instance.FootStepSound();
    }
}
