using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CrownHealthManager : HealthManager
{
    public GameObject model;

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
    public void Respawn()
    {
        model.SetActive(true);
        col.enabled = true;
    }
    protected override void OnDevourEnd(int attackerID)
    {
        col.enabled = false;
        model.SetActive(false);
    }
}
