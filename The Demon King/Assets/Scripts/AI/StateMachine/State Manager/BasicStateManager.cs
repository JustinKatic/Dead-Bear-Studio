using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicStateManager : StateManager
{
    // Start is called before the first frame update
    void Start()
    {
        healthManager = GetComponent<MinionHealthManager>();
        wanderState = GetComponentInChildren<WanderState>();
        chaseState = GetComponentInChildren<ChaseState>();
        attackState = GetComponentInChildren<AttackState>();
        provokedState = GetComponentInChildren<ProvokedState>();
        stunnedState = GetComponentInChildren<StunnedState>();

        currentState = wanderState;
    }

    protected override void RunStateMachine()
    {

        if (healthManager.isStunned)
        {
            SwitchToTheNextState(stunnedState);
            target = null;
        }
        else if (chasing)
        {
            HasBeenChasingPlayerForX();
        }
        else if (healthManager.PlayerWhoShotMe != null)
        {
            provokedState.target = healthManager.PlayerWhoShotMe;
            target = provokedState.target;
            healthManager.PlayerWhoShotMe = null;
            chasing = true;
            SwitchToTheNextState(provokedState);
        }
        else if (CheckIfPlayerIsInMyAttackDistance())
        {
            attackState.target = target;
            SwitchToTheNextState(attackState);
        }
        else if (CheckIfAPlayerIsInMyChaseRadius())
        {
            target = chaseState.target;
            SwitchToTheNextState(chaseState);
        }
        else
        {
            target = null;
            SwitchToTheNextState(wanderState);
        }
        
        base.RunStateMachine();
    }
    
}
