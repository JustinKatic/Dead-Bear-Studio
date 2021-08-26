using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AttackState : State
{
    public bool CanAttack = false;
    public override void RunCurrentState()
    {
        PlayAttackState();
    }

    private void PlayAttackState()
    {
        var towardsPlayer = target.transform.position - minionTransform.position;

        minionTransform.rotation = Quaternion.RotateTowards(
            minionTransform.rotation, 
            Quaternion.LookRotation(towardsPlayer), 
            Time.deltaTime * 180
        );        
        
        agent.SetDestination(target.transform.position + Vector3.one);
        
        if (CanAttack)
        {
            target.GetComponent<PlayerHealthManager>().TakeDamage(1,0 );
        }
    }
    
}
