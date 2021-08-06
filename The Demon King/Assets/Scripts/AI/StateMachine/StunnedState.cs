using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : State
{
    public WanderState WanderState;
    public override State RunCurrentState()
    {
        if (!healthManager.isStunned)
        {
            return WanderState;
        }
        else
        {
            PlayStunnedState();
            return this;
        }
    }

    private void PlayStunnedState()
    {
        anim.SetBool("Walking", false);
        agent.isStopped = true;
        agent.ResetPath();
    }
}
