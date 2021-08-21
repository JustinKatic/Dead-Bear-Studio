using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvokedState : State
{
    public override void RunCurrentState()
    {
        PlayProvokedState();
    }
    
    private void PlayProvokedState()
    {
        anim.SetBool("Walking", true);
        agent.SetDestination(target.transform.position);
    }

}
