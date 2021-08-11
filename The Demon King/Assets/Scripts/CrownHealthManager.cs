using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CrownHealthManager : HealthManager
{
    
    // Start is called before the first frame update
    void Start()
    {
        isStunned = true;
        MaxHealth = 0;
        canBeDevoured = true;
    }

    [PunRPC]
    public void Respawn()
    {
        gameObject.SetActive(true);

    }
    [PunRPC]
    public override void OnDevour(int attackerID)
    {
        gameObject.SetActive(false);

    }
}
