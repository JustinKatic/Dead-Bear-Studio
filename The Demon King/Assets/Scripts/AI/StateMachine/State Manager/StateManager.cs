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
    protected PlayerHealthManager targetHealthManager;
    protected GameObject target;

    //Chase conditions variables
    [SerializeField] protected LayerMask playerLayer;

    [Header("Chasing")]
    [SerializeField] protected float ChasePlayerForX;
    [SerializeField] protected float RadiusDistanceToStartChasingPlayer;
    protected float chaseTimer;
    protected bool chasing = false;

    [Header("Attacking")]
    [SerializeField] protected float TimeTillNextAttack;
    [SerializeField] protected float AttackRange;
    protected float meleeTimer;
    protected bool targetIsStunned = false;
    protected bool targetIsRespawning = false;
    protected bool canMelee = false;

    [Header("Fleeing")]
    [SerializeField] protected int FleeUntilThisHealth;
    protected bool fleeing = false;

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
    protected bool CheckIfAPlayerIsInMyRadius()
    {
        Collider[] colsHit;
        // check with a overlap sphere around the AI
        colsHit = Physics.OverlapSphere(transform.position, RadiusDistanceToStartChasingPlayer, playerLayer);

        foreach (Collider col in colsHit)
        {
            if (col.CompareTag("PlayerParent"))
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
