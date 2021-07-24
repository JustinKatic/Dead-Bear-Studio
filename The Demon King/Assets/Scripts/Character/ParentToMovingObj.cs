using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentToMovingObj : MonoBehaviourPun
{
    //Parent the player to moving enviroment
    private void OnTriggerEnter(Collider other)
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Enviroment"))
                transform.parent.SetParent(other.transform);
        }
    }

    //UnParent the player from moving enviroment
    private void OnTriggerExit(Collider other)
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Enviroment"))
                transform.parent.SetParent(null);
        }
    }
}
