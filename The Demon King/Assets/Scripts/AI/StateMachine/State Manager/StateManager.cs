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
    
    protected MinionHealthManager healthManager;
    
    //Chase conditions variables
    [SerializeField] protected float RadiusDistanceToStartChasingPlayer;
    [SerializeField] protected float AttackRange;
    [SerializeField] protected GameObject target;
    [SerializeField] protected LayerMask playerLayer;
    
    [SerializeField] protected float timer;
    [SerializeField] protected float ChasePlayerForX;
    [SerializeField] protected bool chasing = false;

    
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
        currentState = nextState;
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
                chaseState.target = col.gameObject;
                target = wanderState.target;
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
            timer += Time.deltaTime;
            if (timer >= ChasePlayerForX)
            {
                timer = 0;
                chasing = false;
                //return true;
            }
        }
        //return false;
    }
}
