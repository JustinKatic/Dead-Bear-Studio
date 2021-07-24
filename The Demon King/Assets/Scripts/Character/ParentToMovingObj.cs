using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentToMovingObj : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Enviroment"))
                transform.parent.SetParent(other.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.gameObject.CompareTag("Enviroment"))
                transform.parent.SetParent(null);
        }
    }
}
