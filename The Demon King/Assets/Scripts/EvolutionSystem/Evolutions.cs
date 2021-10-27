using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


[System.Serializable]
public struct AnimationSounds
{
    [FMODUnity.EventRef]
    public string WalkSound;

    [FMODUnity.EventRef]
    public string JumpSound;

    [FMODUnity.EventRef]
    public string DeathByLavaSound;

    [FMODUnity.EventRef]
    public string ShootSound;

    [FMODUnity.EventRef]
    public string FallingSound;

    [FMODUnity.EventRef]
    public string LandingNormalSound;

    [FMODUnity.EventRef]
    public string LandingBigSound;

    [FMODUnity.EventRef]
    public string DevourSound;

    [FMODUnity.EventRef]
    public string StunSound;

    [FMODUnity.EventRef]
    public string EvolveSound;

    [FMODUnity.EventRef]
    public string DemonKingEvolveSound;

    [FMODUnity.EventRef]
    public string DemonKingAnnouncementSound;

    [FMODUnity.EventRef]
    public string RayChargeUpSound;

    [FMODUnity.EventRef]
    public string RayFullyChargedUpShootSound;
}

[System.Serializable]
public class Evolutions : MonoBehaviourPun
{
    public MinionType MyMinionType;
    public int MaxHealth;
    public float ExpWorth;
    public int ScoreWorth;
    public int healthRegenAmount;
    public float TimeTakenToBeDevoured;
    public int AmountToHealAfterStunned;
    public int AmountToHealWhenEvolveing;
    public Material myMatInstance;
    public Sprite MyHealthBarDisplaySprite;




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
