using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class BasicStateManager : StateManager
{
    // Start is called before the first frame update
    void Start()
    {
        healthManager = GetComponent<MinionHealthManager>();
        wanderState = GetComponentInChildren<WanderState>();
        stunnedState = GetComponentInChildren<StunnedState>();
        fleeState = GetComponentInChildren<FleeState>();
        agent = GetComponent<NavMeshAgent>();

        currentState = wanderState;
    }

    protected override void RunStateMachine()
    {

        if (healthManager.CurrentHealth <= healthToFleeAt)
        {
            fleeing = true;
        }
        else
        {
            fleeing = false;
        }

        // Logic for the switching of behaviours at runtime
        if (healthManager.isStunned)
        {
            SwitchToTheNextState(stunnedState);
            target = null;
            wanderState.timer = wanderState.waitAtDestinationTimer;
        }
        else if (fleeing)
        {
            if (CheckIfAPlayerIsInMyRadius())
            {
                fleeState.target = target;
                SwitchToTheNextState(fleeState);
            }
        }
        else
        {
            target = null;
            SwitchToTheNextState(wanderState);
        }
        
        base.RunStateMachine();
    } 
}
