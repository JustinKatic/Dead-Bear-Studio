using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : State
{
    //Wander Variables
    Vector3 dest;
    public bool wanderPosFound;

    public override void RunCurrentState()
    {
        PlayeWanderState();
    }

    public void PlayeWanderState()
    {
        if (wanderPosFound == false)
        {
            dest = RandomPoint(20f);
            agent.isStopped = false;
            wanderPosFound = true;
            anim.SetBool("Walking", true);        }
        
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
    
}
