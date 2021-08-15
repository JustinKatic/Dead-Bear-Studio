using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;


[System.Serializable]
public struct AnimationSounds
{
    [FMODUnity.EventRef]
    public string WalkSound;

    [FMODUnity.EventRef]
    public string JumpSound;

    [FMODUnity.EventRef]
    public string ShootSound;

    [FMODUnity.EventRef]
    public string FallingSound;

    [FMODUnity.EventRef]
    public string LandingSound;
}

[System.Serializable]
public class Evolutions : MonoBehaviourPun
{
    public MinionType MyMinionType;
    public int MaxHealth;


    [FMODUnity.EventRef]
    public string JumpSound;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform ShootPoint;
    public AnimationSounds ModelAnimationSounds;

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
