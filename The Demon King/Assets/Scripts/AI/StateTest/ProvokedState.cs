using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvokedState : State
{
    public ChaseState ChaseState;
    public StunnedState StunnedState;
    public WanderState WanderState;
    public GameObject target;


    public float RadiusDistanceToStartChasingPlayer;

    float timer;
    public float ChasePlayerForX;



    public override State RunCurrentState()
    {
        if (healthManager.isStunned)
        {
            timer = 0;
            return StunnedState;
        }
        else if (CheckIfAPlayerIsInMyChaseRadius())
        {
            return ChaseState;
        }
        else if (HasBeenChasingPlayerForX())
        {
            return WanderState;
        }
        else
        {
            PlayProvokedState();
            return this;
        }
    }


    private void PlayProvokedState()
    {
        agent.SetDestination(target.transform.position);
    }

    bool HasBeenChasingPlayerForX()
    {
        timer += Time.deltaTime;
        if (timer >= ChasePlayerForX)
        {
            timer = 0;
            return true;
        }

        return false;
    }



    bool CheckIfAPlayerIsInMyChaseRadius()
    {
        Collider[] colsHit;
        // check with a overlap sphere around the AI
        colsHit = Physics.OverlapSphere(transform.position, RadiusDistanceToStartChasingPlayer);

        foreach (Collider col in colsHit)
        {
            if (col.CompareTag("Player"))
            {
                ChaseState.target = col.gameObject;
                timer = 0;
                return true;
            }
        }
        return false;
    }
}
