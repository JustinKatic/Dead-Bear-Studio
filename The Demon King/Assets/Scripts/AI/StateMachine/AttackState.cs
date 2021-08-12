using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public WanderState WanderState;
    public StunnedState StunnedState;

    public float AttackRange;
    public GameObject target;


    public override State RunCurrentState()
    {
        if (healthManager.isStunned)
        {
            return StunnedState;
        }
        else if (!CheckIfPlayerIsInMyAttackDistance())
        {
            return WanderState;
        }
        else
        {
            PlayAttackState();
            return this;
        }
    }

    private void PlayAttackState()
    {
        anim.SetBool("Walking", false);
        agent.isStopped = true;
        agent.ResetPath();
    }


    bool CheckIfPlayerIsInMyAttackDistance()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget < AttackRange)
        {
            return true;
        }
        return false;
    }
}
