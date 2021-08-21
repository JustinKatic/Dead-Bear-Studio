using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : State
{
    public override void RunCurrentState()
    {
        PlayStunnedState();
    }

    private void PlayStunnedState()
    {
        anim.SetBool("Walking", false);
        agent.isStopped = true;
        agent.ResetPath();
    }
}
