using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviourPun
{
    [Header("HealthStats")]
    [HideInInspector] public int MaxHealth;
    [SerializeField] protected float TimeBeforeHealthRegen = 3f;
    [SerializeField] protected int healthRegenAmount = 1;
    [SerializeField] protected float healthRegenTickrate = .5f;
    [SerializeField] protected float RespawnTime = 6;
    [SerializeField] protected int AmountOfHealthAddedAfterStunned = 3;


    [SerializeField] private float stunnedDuration = 3;

    public float TimeTakenToBeDevoured;
    public float StunnedDuration { get { return stunnedDuration; } private set { stunnedDuration = value; } }

    [Header("Evolution Stats")]
    public int MyExperienceWorth = 2;
    public int myScoreWorth = 5;

    [HideInInspector] public MinionType MyMinionType;

    [Header("healthBar Health UI IMG")]
    [SerializeField] protected Image healthBarPrefab;

    [Header("Overhead healthBar Hud")]
    [SerializeField] protected Canvas overheadHealthBar;

    [Header("VFX Effects")]
    [SerializeField] protected GameObject StunVFX;


    public float healthRegenTimer = 0f;
    public float timeForHealthRegenToActivate = 8f;


    protected float healthRegenTickrateTimer = 0f;


    protected float stunnedTimer;
    public int CurAttackerId;

    public int CurrentHealth = 0;
    [HideInInspector] public bool beingDevoured = false;
    public bool canBeDevoured = false;
    [HideInInspector] public bool isStunned = false;

    protected float currentHealthOffset = 0;
    [SerializeField] protected float healthOffsetTime = 5;

    protected IEnumerator myDevourCo;

    #region Update Loops
    virtual protected void Update()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            if (isStunned)
            {
                stunnedTimer += Time.deltaTime;
                if (stunnedTimer >= StunnedDuration)
                {
                    if (!beingDevoured)
                    {
                        OnBeingStunnedEnd();
                        stunnedTimer = 0;
                    }
                }
            }

            if (beingDevoured || isStunned)
                return;
            //Heal every X seconds if not at max health
            if ((healthRegenTimer < timeForHealthRegenToActivate) && (!beingDevoured || !isStunned))
            {
                healthRegenTimer += Time.deltaTime;
            }
            if (CurrentHealth < MaxHealth && healthRegenTimer >= timeForHealthRegenToActivate)
            {
                healthRegenTickrateTimer += Time.deltaTime;
                if (healthRegenTickrateTimer >= healthRegenTickrate)
                {
                    Heal(healthRegenAmount);
                    healthRegenTickrateTimer = 0;
                }
            }
        }
    }
    #endregion

    #region Devour

    public void OnDevour(int attackerID)
    {
        photonView.RPC("OnDevour_RPC", RpcTarget.All, attackerID);
    }

    [PunRPC]
    public virtual void OnDevour_RPC(int attackerID)
    {
        if (CurAttackerId == 0)
        {
            CurAttackerId = attackerID;
            myDevourCo = DevourCorutine();
            StartCoroutine(myDevourCo);
        }
        else
        {
            InteruptDevourOnPersonDevouring();
        }

        IEnumerator DevourCorutine()
        {
            OnBeingDevourStart();

            yield return new WaitForSeconds(TimeTakenToBeDevoured);
            CurAttackerId = 0;
            OnBeingDevourEnd(attackerID);
        }
    }

    //Overrides for inherited classes
    protected virtual void OnBeingDevourStart()
    {

    }

    protected virtual void OnBeingDevourEnd(int attackerID)
    {

    }

    public void InteruptDevourOnPersonDevouring()
    {
        photonView.RPC("InterruptDevourOnSelf_RPC", RpcTarget.All);
    }

    [PunRPC]
    protected virtual void InteruptDevourOnPersonDevouring_RPC()
    {

    }



    public void InterruptDevourOnSelf()
    {
        photonView.RPC("InterruptDevourOnSelf_RPC", RpcTarget.All);
    }

    [PunRPC]
    protected virtual void InterruptDevourOnSelf_RPC()
    {
        beingDevoured = false;
        CurAttackerId = 0;
        StopCoroutine(myDevourCo);

        if (photonView.IsMine)
        {
            OnBeingStunnedEnd();
            stunnedTimer = 0;
        }
    }

    #endregion

    #region Stun

    //Overrides for inherited classes
    protected virtual void OnBeingStunnedStart()
    {

    }

    protected virtual void OnBeingStunnedEnd()
    {

    }

    #endregion

    #region HealthFunctions
    virtual protected void Heal(int amountToHeal)
    {

    }
    #endregion
}
