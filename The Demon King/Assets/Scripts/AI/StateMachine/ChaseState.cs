using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public AttackState AttackState;
    public StunnedState StunnedState;
    public WanderState WanderState;
    public float AttackRange;
    public float ChaseRange;
    public GameObject target;

    public override State RunCurrentState()
    {
        if (healthManager.isStunned)
        {
            return StunnedState;
        }
        else if (CheckIfPlayerLeftChaseDistance() || target.gameObject == null)
        {
            return WanderState;
        }
        else if (CheckIfPlayerIsInMyAttackDistance())
        {
            return AttackState;
        }
        else
        {
            PlayChaseState();
            return this;
        }
    }


    private void PlayChaseState()
    {
        anim.SetBool("Walking", true);
        agent.SetDestination(target.transform.position);
    }

    bool CheckIfPlayerLeftChaseDistance()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget > ChaseRange)
        {
            return true;
        }
        return false;
    }


    bool CheckIfPlayerIsInMyAttackDistance()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget < AttackRange)
        {
            AttackState.target = target;
            return true;
        }
        return false;
    }
}
