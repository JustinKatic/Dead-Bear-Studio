using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Wandering : MonoBehaviourPun
{
    private NavMeshAgent agent;
    Vector3 location;
    private float distanceToDestination = 0;
    private Vector3 previousDestination;
    HealthManager healthManager;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            anim = GetComponentInChildren<Animator>();
            agent = GetComponent<NavMeshAgent>();
            healthManager = GetComponent<HealthManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //If the AI has not been stunned move to next position
            if (!healthManager.isStunned)
            {
                distanceToDestination = Vector3.Distance(gameObject.transform.position, location);

                if (location == Vector3.zero || previousDestination == location)
                {
                    anim.SetBool("Walking", true);
                    location = RandomNavSphere(gameObject.transform.position, 10, -1);
                    agent.SetDestination(location);
                }

                if (distanceToDestination <= 1)
                {
                    location = RandomNavSphere(gameObject.transform.position, 10, -1);
                    agent.SetDestination(location);
                }

                if (agent.velocity == Vector3.zero)
                {
                    location = RandomNavSphere(gameObject.transform.position, 10, -1);
                    agent.SetDestination(location);
                }
            }
            else
            {
                //Sets the position of the next movement to it's current position when stunned
                distanceToDestination = 0;
                location = transform.position;
                agent.SetDestination(location);
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;

    }
}
