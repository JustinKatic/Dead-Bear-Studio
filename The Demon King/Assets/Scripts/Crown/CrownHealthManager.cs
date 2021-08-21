using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CrownHealthManager : HealthManager
{
    public GameObject model;
    public GameObject VFX;


    private Collider col;
    // Start is called before the first frame update
    void Start()
    {
        isStunned = true;
        MaxHealth = 0;
        canBeDevoured = true;
        col = GetComponent<Collider>();
    }

    [PunRPC]
    public void CrownRespawn()
    {
        model.SetActive(true);
        VFX.SetActive(true);
        col.enabled = true;
    }


    [PunRPC]
    public void DespawnCrown()
    {
        col.enabled = false;
        model.SetActive(false);
        VFX.SetActive(false);
    }

    protected override void OnBeingDevourEnd(int attackerID)
    {
        photonView.RPC("DespawnCrown", RpcTarget.All);
    }
}
