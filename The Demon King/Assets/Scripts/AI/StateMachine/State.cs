using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public abstract class State : MonoBehaviour
{
    public abstract State RunCurrentState();
    protected NavMeshAgent agent;
    protected MinionHealthManager healthManager;
    protected Animator anim;

    private void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        healthManager = GetComponentInParent<MinionHealthManager>();
        anim = GetComponentInParent<Animator>();
    }
}
