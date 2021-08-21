using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public override void RunCurrentState()
    {
        PlayChaseState();
    }
    
    private void PlayChaseState()
    {
        anim.SetBool("Walking", true);
        agent.SetDestination(target.transform.position);
    }
    
}
