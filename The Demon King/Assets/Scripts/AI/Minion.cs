using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

public class Minion : MonoBehaviourPun
{
    public bool dead = false;
    public Vector3 spawnPos;
    
    public void Death()
    {
        dead = true;
        
        gameObject.SetActive(false);       
    }

    private void OnEnable()
    {
        spawnPos = transform.position;
    }

    [PunRPC]
    public void RespawnThisMinion()
    {
        transform.position = spawnPos;
    }

}
