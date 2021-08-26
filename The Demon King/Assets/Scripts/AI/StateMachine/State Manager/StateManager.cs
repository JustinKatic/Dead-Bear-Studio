using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StateManager : MonoBehaviourPun
{
    public State currentState;
    
    protected WanderState wanderState;
    protected ChaseState chaseState;
    protected StunnedState stunnedState;
    protected AttackState attackState;
    protected ProvokedState provokedState;
    protected FleeState fleeState;
    
    protected MinionHealthManager healthManager;
    
    //Chase conditions variables
    [SerializeField] protected float RadiusDistanceToStartChasingPlayer;
    [SerializeField] protected float AttackRange;
    [SerializeField] protected GameObject target;
    [SerializeField] protected PlayerHealthManager targetHealthManager;
    [SerializeField] protected LayerMask playerLayer;
    
    protected float meleeTimer;
    protected float chaseTimer;
    [SerializeField] protected int FleeUntilThisHealth;
    [SerializeField] protected float ChasePlayerForX;
    [SerializeField] protected bool chasing = false;
    [SerializeField] protected float TimeTillNextAttack;
    [SerializeField] protected bool canMelee = false;
    [SerializeField] protected bool targetIsStunned = false;
    [SerializeField] protected bool targetBeingDevoured = false;
    [SerializeField] protected bool fleeing = false;


    
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            RunStateMachine();
    }

    protected virtual void RunStateMachine()
    {
        currentState?.RunCurrentState(); 

    }

    protected void SwitchToTheNextState(State nextState)
    {

        if (nextState != currentState)
        {
            currentState = nextState;
            chaseTimer = 0;
            meleeTimer = 0;
        }

    }
    protected bool CheckIfAPlayerIsInMyChaseRadius()
    {
        Collider[] colsHit;
        // check with a overlap sphere around the AI
        colsHit = Physics.OverlapSphere(transform.position, RadiusDistanceToStartChasingPlayer);

        foreach (Collider col in colsHit)
        {
            if (col.CompareTag("Player"))
            {
                target = col.gameObject;
                wanderState.wanderPosFound = false;
                return true;
            }
        }
        return false;
    }
    
    protected bool CheckIfPlayerIsInMyAttackDistance()
    {
        if (target == null)
            return false;

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget < AttackRange)
        {
            return true;
        }
        return false;
    }
    
    protected void HasBeenChasingPlayerForX()
    {
        if (chasing)
        {
            chaseTimer += Time.deltaTime;
            if (chaseTimer >= ChasePlayerForX)
            {
                chaseTimer = 0;
                chasing = false;
            }
        }
    }
    
    protected bool CanAttackAfterTime()
    {
        meleeTimer += Time.deltaTime;
        if (meleeTimer >= TimeTillNextAttack)
        {
            meleeTimer = 0;
            return true;
        }
        return false;
    }
    
}
