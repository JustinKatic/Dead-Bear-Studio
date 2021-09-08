using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AdvancedStateManager : StateManager
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
        agent = GetComponent<NavMeshAgent>();

        currentState = wanderState;
    }
    protected override void RunStateMachine()
    {
          //if the target has been found check if they have been stunned
        //Assigned is stunned to a seperate bool to avoid null references
        if(target != null)
        {
            targetHealthManager = target.GetComponent<PlayerHealthManager>();
            
            targetIsStunned = targetHealthManager.isStunned;
            targetIsRespawning = targetHealthManager.isRespawning;
        }
        else
        {
            targetHealthManager = null;
            targetIsRespawning = false;
            targetIsStunned = false;
        }

        if (healthManager.CurrentHealth <= FleeAtThisHealth)
        {
            fleeing = true;
        }
        else if (healthManager.CurrentHealth >= FleeUntilThisHealth)
        {
            fleeing = false;
        }
        
        // Logic for the switching of behaviours at runtime
        //My target is stunned or being devoured so I need to wander
        if (targetIsStunned || targetIsRespawning)
        {
            SwitchToTheNextState(wanderState);
            target = null;
        }
        //I am in a stunned state
        else if (healthManager.CurrentHealth <= 0)
        {
            SwitchToTheNextState(stunnedState);

            target = null;
        }
        // Health is low and a player is in my near vicinity, Flee
        else if (fleeing)
        {
            if (CheckIfAPlayerIsInMyRadius())
            {
                fleeState.target = target;
                SwitchToTheNextState(fleeState);
            }

        }
        //I have been shot and I am noy currently fleeing, Provoked state
        else if (healthManager.PlayerWhoShotMe != null && !fleeing)
        {
            provokedState.target = healthManager.PlayerWhoShotMe;
            target = provokedState.target;
            healthManager.PlayerWhoShotMe = null;
            chasing = true;
            SwitchToTheNextState(provokedState);
        }
        // Player is in my chase radius, Chase State
        else if (CheckIfAPlayerIsInMyRadius())
        {
            //Player is in my attack range, Attack State
            if (CheckIfPlayerIsInMyAttackDistance())
            {
                attackState.target = target;
                attackState.CanAttack = CanAttackAfterTime();
                SwitchToTheNextState(attackState);
            }
            else
            {
                chaseState.target = target;

                SwitchToTheNextState(chaseState);
            }

        }
        //Player is out of my range but I am chasing for time frame, so Keep chasing for this time
        else if (chasing && !targetIsStunned)
        {
            HasBeenChasingPlayerForX();
        }
        //Wander
        else
        {
            target = null;
            SwitchToTheNextState(wanderState);
        }
        
        base.RunStateMachine();
    } 
}
