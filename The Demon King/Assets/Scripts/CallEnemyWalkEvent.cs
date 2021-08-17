using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CallEnemyWalkEvent : MonoBehaviourPun
{
    private void FootStepSound()
    {
        if (photonView.IsMine)
            Debug.Log("PlayMinionWalkSound");
    }
}
