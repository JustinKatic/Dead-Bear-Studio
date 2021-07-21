using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MinionHealthManager : HealthManager
{
    private Wandering aiMove;
    private Minion minion;
   private void Awake()
   {
        minion = GetComponent<Minion>();
        aiMove = GetComponent<Wandering>();
        CurrentHealth = MaxHealth;
        OverheadText.text = CurrentHealth.ToString();
    }

    private void Update()
    {
        if (CurrentHealth < MaxHealth)
        {
            HealthRegenTimer -= Time.deltaTime;
            if (HealthRegenTimer <= 0)
            {
                photonView.RPC("Heal", RpcTarget.All, 1);
            }
        }
    }
    protected override void StunnedBehaviour()
    {
        //Things that only affect local
        if (photonView.IsMine)
        {
            aiMove.stunned = true;
        }
    }

    protected override void DevourFinished()
    {
        minion.Death();

    }

    protected override void InterruptedDevour()
    {
        base.InterruptedDevour();
    }
}
