using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MinionHealthManager : HealthManager
{
    AIRespawn aiRespawner;

    private IEnumerator myDevourCo;



    protected override void Awake()
    {
        base.Awake();
        aiRespawner = GetComponentInParent<AIRespawn>();
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
