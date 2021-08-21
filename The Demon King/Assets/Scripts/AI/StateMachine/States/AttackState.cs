using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override void RunCurrentState()
    {
        PlayAttackState();
    }

    private void PlayAttackState()
    {
        anim.SetBool("Walking", false);
        agent.isStopped = true;
        agent.ResetPath();
    }

}
