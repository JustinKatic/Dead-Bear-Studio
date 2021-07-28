using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;

public class PlayerNameTag : MonoBehaviourPun
{
    [SerializeField] private TMP_Text nameText;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            SetName();
        }
        else
        {
            Destroy(nameText.gameObject);
        }
    }

    private void SetName()
    {
        nameText.text = photonView.Owner.NickName;
    }
}
