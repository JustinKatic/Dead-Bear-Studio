using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Wandering : Behaviour
{
    Vector3 dest;
    private float distanceToDestination = 0;
    private bool wanderPosFound;

    // Update is called once per frame
    public override void RunBehaviour()
    {
        if (photonView.IsMine)
        {
            //If the AI has not been stunned move to next position
            if (!healthManager.isStunned)
            {
                if (wanderPosFound == false)
                {
                    dest = RandomPoint(20f);
                    agent.isStopped = false;
                    wanderPosFound = true;
                }
                distanceToDestination = Vector3.Distance(gameObject.transform.position, dest);
                agent.SetDestination(dest);

                if (distanceToDestination <= 2)
                    wanderPosFound = false;
            }
            else
            {
                //Sets the position of the next movement to it's current position when stunned
                anim.SetBool("Walking", false);
                agent.isStopped = true;
                agent.ResetPath();
            }
        }
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
