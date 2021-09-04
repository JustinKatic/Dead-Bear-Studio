using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;


public abstract class State : MonoBehaviourPun
{
    public abstract void RunCurrentState();
    protected NavMeshAgent agent;
    protected Transform minionTransform;
    protected Animator anim;
    [HideInInspector] public GameObject target;
    protected EnemySoundManager enemySoundManager;
    protected MinionHealthManager minionHealthManager;

    private void Awake()
    {
        agent = GetComponentInParent<NavMeshAgent>();
        minionTransform = agent.GetComponent<Transform>();
        anim = GetComponentInParent<Animator>();
        enemySoundManager = GetComponentInParent<EnemySoundManager>();
        minionHealthManager = GetComponentInParent<MinionHealthManager>();
    }
}
