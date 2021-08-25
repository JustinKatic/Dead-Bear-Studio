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
        agent.SetDestination(target.transform.position + Vector3.one);
        
        if (CanAttack)
        {
            target.GetComponent<PlayerHealthManager>().TakeDamage(1,0 );
        }
    }
    
}
