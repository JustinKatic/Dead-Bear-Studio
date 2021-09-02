using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SetEmoteInactive : MonoBehaviourPun
{
    private Animation floatingAnimation;

    // Start is called before the first frame update
    void Start()
    {
        floatingAnimation = GetComponentInChildren<Animation>();
    }

    private void Update()
    {
        
        if (!floatingAnimation.isPlaying)
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
