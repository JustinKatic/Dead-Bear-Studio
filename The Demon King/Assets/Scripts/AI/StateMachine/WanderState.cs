using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : State
{
    public ChaseState ChaseState;
    public StunnedState StunnedState;
    public ProvokedState ProvokedState;

    //Chase conditions variables
    public float RadiusDistanceToStartChasingPlayer;
    public LayerMask playerLayer;

    //Wander Variables
    Vector3 dest;
    private bool wanderPosFound;

    public override State RunCurrentState()
    {
        if (healthManager.isStunned)
        {
            return StunnedState;
        }
        else if (healthManager.PlayerWhoShotMe != null)
        {
            ProvokedState.target = healthManager.PlayerWhoShotMe;
            healthManager.PlayerWhoShotMe = null;
            return ProvokedState;
        }
        else if (CheckIfAPlayerIsInMyChaseRadius())
        {
            
            return ChaseState;
        }
        else
        {
            PlayWanderState();
            return this;
        }
    }


    public void PlayWanderState()
    {
        if (wanderPosFound == false)
        {
            dest = RandomPoint(20f);
            agent.isStopped = false;
            wanderPosFound = true;
        }
        float distanceToDestination = Vector3.Distance(gameObject.transform.position, dest);
        agent.SetDestination(dest);

        if (distanceToDestination <= 2)
            wanderPosFound = false;
    }



    public Vector3 RandomPoint(float range)
    {
        Vector3 result = Vector3.zero;
        bool searching = true;

        while (searching)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(hit.position, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    result = hit.position;
                    searching = false;
                }
            }
        }
        return result;
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
                wanderPosFound = false;
                return true;
            }
        }
        return false;
    }
}
