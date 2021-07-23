using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MinionHealthManager : HealthManager
{
    AIRespawn aiRespawner;


    private void Awake()
    {
        CurrentHealth = MaxHealth;
        SetStatusStartValues();
        //OverheadText.text = CurrentHealth.ToString();
        aiRespawner = GetComponentInParent<AIRespawn>();
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

            aiRespawner.Respawn();
            gameObject.SetActive(false);
        }
    }
    protected override void OnStunStart()
    {
        //Things that affect everyone
        canBeDevoured = true;

        //Things that only affect local
        if (photonView.IsMine)
        {
            isStunned = true;
            photonView.RPC("ChangeStatusBarStun", RpcTarget.All);
        }
    }

    protected override void OnStunEnd()
    {
        if (!beingDevoured)
        {
            //Things that affect everyone
            canBeDevoured = false;

            //Things that only affect local
            if (photonView.IsMine)
            {
                isStunned = false;
                photonView.RPC("ChangeStatusBarHealth", RpcTarget.All);
            }
        }
    }

    protected override void InterruptedDevour()
    {
        base.InterruptedDevour();
    }
}
