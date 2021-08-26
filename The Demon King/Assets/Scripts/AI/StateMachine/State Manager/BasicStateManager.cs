using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        fleeState = GetComponentInChildren<FleeState>();

        currentState = wanderState;
    }

    protected override void RunStateMachine()
    {
        //if the target has been found check if they have been stunned
        //Assigned is stunned to a seperate bool to avoid null references
        if(target != null)
        {
            if (targetHealthManager == null)
            {
                targetHealthManager = target.GetComponent<PlayerHealthManager>();
            }
            targetIsStunned = targetHealthManager.isStunned;
            targetBeingDevoured = targetHealthManager.beingDevoured;
        }
        else
        {
            targetBeingDevoured = false;
            targetIsStunned = false;
        }

        if (healthManager.CurrentHealth == 1)
        {
            fleeing = true;
        }
        else if (healthManager.CurrentHealth >= FleeUntilThisHealth)
        {
            fleeing = false;
        }

        // Logic for the switching of behaviours at runtime
        if (healthManager.CurrentHealth <= 0)
        {
            SwitchToTheNextState(stunnedState);

        }
        else if (targetIsStunned)
        {
            SwitchToTheNextState(wanderState);

        }
        else if (healthManager.isStunned)
        {
            SwitchToTheNextState(stunnedState);

            target = null;
        }
        else if (fleeing && CheckIfAPlayerIsInMyChaseRadius())
        {
            fleeState.target = target;
            SwitchToTheNextState(fleeState);

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
            attackState.CanAttack = CanAttackAfterTime();
            SwitchToTheNextState(attackState);
        }
        else if (CheckIfAPlayerIsInMyChaseRadius())
        {
            chaseState.target = target;

            SwitchToTheNextState(chaseState);
        }
        else if (chasing && !targetIsStunned)
        {
            HasBeenChasingPlayerForX();
        }
        else
        {
            target = null;
            SwitchToTheNextState(wanderState);
        }
        
        base.RunStateMachine();
    } 
}
