using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MinionHealthManager : HealthManager
{
    AISpawner aISpawner;

   private void Awake()
   {
        CurrentHealth = MaxHealth;
        OverheadText.text = CurrentHealth.ToString();
        aISpawner = GetComponent<AISpawner>();
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


    [PunRPC]
    void OnDevour()
    {
        if (!photonView.IsMine)
            return;

        StartCoroutine(DevourCorutine());
        IEnumerator DevourCorutine()
        {
            beingDevoured = true;
            Debug.Log("NONNONONON");

            yield return new WaitForSeconds(DevourTime);

            photonView.RPC("Respawn", RpcTarget.All);
        }
    }
    protected override void OnStunStart()
    {
        //Things that only affect local
        if (photonView.IsMine)
        {
            isStunned = true;
        }
    }

    protected override void OnStunEnd()
    {

    }



    protected override void InterruptedDevour()
    {
        base.InterruptedDevour();
    }
}
