using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class DemonKingCrown : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PhotonView>().RPC("CrownPickedUp",RpcTarget.All);
                other.GetComponent<DemonKingEvolution>().ChangeToTheDemonKing();
            }
        }

    }

    [PunRPC]
    public void CrownPickedUp()
    {
        gameObject.SetActive(false);
    }
}
