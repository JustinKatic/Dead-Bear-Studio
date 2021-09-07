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
        else if (healthManager.isStunned)
        {
            SwitchToTheNextState(stunnedState);

            target = null;
        }
        else if (fleeing && CheckIfAPlayerIsInMyRadius())
        {
            fleeState.target = target;
            SwitchToTheNextState(fleeState);
        }
        else
        {
            target = null;
            SwitchToTheNextState(wanderState);
        }
        
        base.RunStateMachine();
    } 
}
