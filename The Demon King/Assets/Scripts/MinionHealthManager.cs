using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class MinionHealthManager : HealthManager
{
    AIRespawn aiRespawner;

    private IEnumerator myDevourCo;



    void Awake()
    {
      //statusBar = GetComponentInChildren<Slider>();
        CurrentHealth = MaxHealth;
        photonView.RPC("UpdateStatusbarValues", RpcTarget.All);

        aiRespawner = GetComponentInParent<AIRespawn>();
    }

    protected override void Heal(int amountToHeal)
    {
        //Only running on local player
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        //Updates this charcters status bar on all players in network
        photonView.RPC("UpdateStatusBar", RpcTarget.Others, CurrentHealth);
        // statusBar.value = CurrentHealth;
        HealthRegenTimer = TimeBeforeHealthRegen;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            //Return if already being devoured
            if (beingDevoured)
                return;

            //Remove health
            CurrentHealth -= damage;
            //Reset health regen timer
            HealthRegenTimer = TimeBeforeHealthRegen;

            //Updates this charcters status bar on all players in network
            photonView.RPC("UpdateStatusBar", RpcTarget.Others, CurrentHealth);

            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                photonView.RPC("Stunned", RpcTarget.All);
        }
    }

    [PunRPC]
    void OnDevour()
    {
        if (!photonView.IsMine)
            return;

        myDevourCo = DevourCorutine();
        StartCoroutine(myDevourCo);

        IEnumerator DevourCorutine()
        {
            beingDevoured = true;
            Debug.Log("NONNONONON");

            yield return new WaitForSeconds(DevourTime);

            aiRespawner.Respawn();
            isStunned = false;
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
            Debug.Log("Play Stun Anim");
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
                Debug.Log("Stop Playing Stun Anim");
            }
        }
    }

    [PunRPC]
    protected override void InterruptDevourOnSelf()
    {
        beingDevoured = false;
        StopCoroutine(myDevourCo);
    }
}
