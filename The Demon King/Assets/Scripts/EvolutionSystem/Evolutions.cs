using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class Evolutions : MonoBehaviourPun
{
    public MinionType MyType;
    public int MaxHealth;

    public Animator animator;
    public Transform ShootPoint;


    private void Awake()
    {
        if (photonView.IsMine)
        {
            //animator on this model
            animator = GetComponent<Animator>();
            //Shoot point on this model
            ShootPoint = transform.Find("ShootPoint").transform;
        }
    }
}
