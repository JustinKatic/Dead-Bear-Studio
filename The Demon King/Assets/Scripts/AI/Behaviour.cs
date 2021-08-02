using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Behaviour : MonoBehaviourPun
{
    protected NavMeshAgent agent;
    protected HealthManager healthManager;
    protected Animator anim;
    public AISpawner mySpawnAreaManager;

    private void Start()
    {
        if (photonView.IsMine)
        {
            anim = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            healthManager = GetComponent<HealthManager>();
        }    
    }

    public virtual void RunBehaviour()
    {
        
    }
}
