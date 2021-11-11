using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : State
{
    //Wander Variables
    NavMeshPath dest;
    public bool wanderPosFound;
    public float timer = 0;
    public float waitAtDestinationTimer = 2f;
    private bool waitAtLocation = true;

    private Vector3 defaultPos;

    private void Start()
    {
        defaultPos = transform.position;
    }

    public override void RunCurrentState()
    {
        PlayWanderState();
    }

    public void PlayWanderState()
    {
        if (!wanderPosFound || minionHealthManager.Respawned)
        {
            dest = RandomPoint(10f);
            anim.SetBool("Walking", false);
        }

        if (waitAtLocation)
        {
            //if timer is less than waitTime
            if (timer < waitAtDestinationTimer)
            {
                //add Time.deltaTime each time we hit this point
                timer += Time.deltaTime;
            }
            //no longer waiting because timer is greater than 10
            else
            {
                //agent.isStopped = false;
                waitAtLocation = false;
                anim.SetBool("Walking", true);
                minionHealthManager.Respawned = false;
                agent.SetPath(dest);
                timer = 0;
            }
        }
        if (agent.remainingDistance <= 0.2f && !waitAtLocation)
        {
            waitAtLocation = true;
            wanderPosFound = false;
        }
    }
    public NavMeshPath RandomPoint(float range)
    {
        bool searching = true;
        NavMeshPath path = new NavMeshPath();
        int numberOfChecks = 0;
        while (searching && numberOfChecks <= 20)
        {
            numberOfChecks++;
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                agent.CalculatePath(hit.position, path);
                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    searching = false;
                }
            }
            if (numberOfChecks >= 20)
            {
                if (NavMesh.SamplePosition(defaultPos, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.CalculatePath(hit.position, path);
                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        searching = false;
                    }
                }
            }
        }

        wanderPosFound = true;

        return path;
    }
}
