using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Wandering : MonoBehaviourPun
{
    private NavMeshAgent agent;
    Vector3 dest;
    private float distanceToDestination = 0;
    HealthManager healthManager;

    private bool wanderPosFound;

    private Animator anim;

    public AISpawner mySpawnAreaManager;

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
                if (wanderPosFound == false)
                {
                    dest = mySpawnAreaManager.RandomPoint(mySpawnAreaManager.transform.position, mySpawnAreaManager.RadiusCheck);
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
}
