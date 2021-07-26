using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestorySelfAfterX : MonoBehaviourPun
{
    public float DestroySelfTime = 5f;
    private void OnEnable()
    {
        if (photonView.IsMine)
            Invoke("DestroySelf", 3f);
    }

    void DestroySelf()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
