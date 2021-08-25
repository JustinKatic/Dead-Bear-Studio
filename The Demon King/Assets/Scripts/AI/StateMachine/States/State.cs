using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;


public abstract class State : MonoBehaviourPun
{
    public abstract void RunCurrentState();
    protected NavMeshAgent agent;
    protected Animator anim;
    [HideInInspector]public GameObject target;

    private void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        anim = GetComponentInParent<Animator>();       
    }
}
