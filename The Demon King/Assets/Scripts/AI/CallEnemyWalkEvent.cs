using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CallEnemyWalkEvent : MonoBehaviourPun
{
    EnemySoundManager enemySoundManager;

    private void Start()
    {
        enemySoundManager = GetComponent<EnemySoundManager>();
    }
    private void FootStepSound()
    {
        enemySoundManager.PlayFootStepSound();
    }
}
