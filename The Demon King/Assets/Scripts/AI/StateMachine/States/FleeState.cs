using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeState : State
{

    public override void RunCurrentState()
    {
        anim.SetBool("Walking", true);
        minionTransform.rotation = Quaternion.LookRotation(minionTransform.position - target.transform.position);
        Vector3 runTo = minionTransform.position + minionTransform.forward * 2;
        agent.SetDestination(runTo);    
    }
}
