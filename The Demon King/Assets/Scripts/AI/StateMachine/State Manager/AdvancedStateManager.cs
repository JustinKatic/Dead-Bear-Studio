using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedStateManager : StateManager
{
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
        }
        else
        {
            targetIsStunned = false;
        }
        
        // Logic for the switching of behaviours at runtime
        if (targetIsStunned)
        {
            SwitchToTheNextState(wanderState);

        }
        else if (healthManager.isStunned)
        {
            SwitchToTheNextState(stunnedState);

            target = null;
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
            target = chaseState.target;

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
