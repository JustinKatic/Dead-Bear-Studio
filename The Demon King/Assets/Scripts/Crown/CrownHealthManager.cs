using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CrownHealthManager : HealthManager
{
    public GameObject model;
    public GameObject VFX;
    private Collider col;

    void Start()
    {
        isStunned = true;
        col = GetComponent<Collider>();
        canBeDevoured = true;
    }


    protected override void OnBeingDevourEnd(int attackerID)
    {
        CrownRespawn(false);
    }



    public void CrownRespawn(bool ShouldRespawn)
    {
        photonView.RPC("CrownRespawn_RPC", RpcTarget.All, ShouldRespawn);
    }

    [PunRPC]
    public void CrownRespawn_RPC(bool ShouldRespawn)
    {
        model.SetActive(ShouldRespawn);
        VFX.SetActive(ShouldRespawn);
        col.enabled = ShouldRespawn;
        canBeDevoured = true;
    }
}
