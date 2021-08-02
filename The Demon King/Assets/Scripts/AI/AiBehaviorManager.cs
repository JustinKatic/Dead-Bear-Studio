using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

public class AiBehaviorManager : MonoBehaviourPun
{
    private Wandering wander;
    private ChaseTarget chaseTarget;
    private Attack attack;

    private float radiusCheck = 5;
    private float attackRange = 1;

    public GameObject Target;

    private Behaviour activeBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        wander = GetComponent<Wandering>();
        chaseTarget = GetComponent<ChaseTarget>();
        attack = GetComponent<Attack>();

        activeBehaviour = wander;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            //Run the Active Behaviour
            activeBehaviour.RunBehaviour();

            //If a player is in my radius change to chasing the target
            if (CheckIfPlayerIsInMyChaseRadius() && !CheckIfPlayerIsInMyAttackDistance())
            {
                //chaseTarget.Target = Target;
                //ChangeBehaviour(chaseTarget);
            }
            else if (CheckIfPlayerIsInMyChaseRadius() && CheckIfPlayerIsInMyAttackDistance())
            {
                //attack.Target = Target;
                //ChangeBehaviour(attack);
            }
            else if (activeBehaviour != wander)
            {
                ChangeBehaviour(wander);
            }
        }
    }

    void ChangeBehaviour(Behaviour newBehaviour)
    {
        activeBehaviour.enabled = false;
        activeBehaviour = newBehaviour;
        activeBehaviour.enabled = true;
    }

    bool CheckIfPlayerIsInMyChaseRadius()
    {
        RaycastHit hit;

        // check with a spherecast if around the AI
        if (Physics.SphereCast(transform.position, radiusCheck, transform.forward, out hit, 10))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Target = hit.collider.gameObject;
                return true;
            }
        }
        return false;
    }


    bool CheckIfPlayerIsInMyAttackDistance()
    {
        float distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);

        if (distanceToTarget < attackRange)
        {
            return true;
        }
        return false;
    }
}
