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
    [SerializeField] protected int MaxHealth = 3;
    [SerializeField] protected float TimeBeforeHealthRegen = 3f;
    [SerializeField] protected float RespawnTime = 6;
    [SerializeField] protected int AmountOfHealthAddedAfterStunned = 3;


    [SerializeField] private float devourTime = 3;
    [SerializeField] private float stunnedDuration = 3;

    public float DevourTime { get { return devourTime; } private set { devourTime = value; } }
    public float StunnedDuration { get { return stunnedDuration; } private set { stunnedDuration = value; } }

    [Header("Evolution Stats")]
    public int MyExperienceWorth = 2;
    [HideInInspector] public MinionType MyMinionType;

    [Header("healthBar Health UI IMG")]
    [SerializeField] protected Image healthBarPrefab;

    [Header("Overhead healthBar Hud")]
    [SerializeField] protected Transform HealthBarContainerOverhead;
    [SerializeField] protected Canvas overheadHealthBar;

    [Header("VFX Effects")]
    [SerializeField] protected GameObject StunVFX;


    protected List<Image> healthBars = new List<Image>();
    protected List<Image> healthBarsOverhead = new List<Image>();
    protected float healthRegenTimer = 3f;
    protected float stunnedTimer;
    protected int curAttackerId;

    public int CurrentHealth = 0;
    [HideInInspector] public bool beingDevoured = false;
    [HideInInspector] public bool canBeDevoured = false;
    [HideInInspector] public bool isStunned = false;

    protected IEnumerator myDevourCo;

    #region Update Loops
    private void Update()
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
            if (CurrentHealth < MaxHealth)
            {
                healthRegenTimer -= Time.deltaTime;
                if (healthRegenTimer <= 0)
                {
                    if (!beingDevoured || !isStunned)
                        Heal(1);
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
        myDevourCo = DevourCorutine();
        StartCoroutine(myDevourCo);

        IEnumerator DevourCorutine()
        {
            OnBeingDevourStart();

            yield return new WaitForSeconds(DevourTime);

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

    public void InterruptDevourOnSelf()
    {
        photonView.RPC("InterruptDevourOnSelf_RPC", RpcTarget.All);
    }

    [PunRPC]
    protected virtual void InterruptDevourOnSelf_RPC()
    {
        beingDevoured = false;
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
    protected void FillBarsOfHealth(int currentHealth, List<Image> bar)
    {
        for (int i = 0; i < MaxHealth; i++)
        {
            //Change health bar red if the bar we are looking at is < currentHealth
            if (i < currentHealth)
                bar[i].color = Color.red;
            //Change health bar transparent if the bar we are looking at is > currentHealth
            else
                bar[i].color = new Color(255, 0, 0, 0);
        }
    }
    protected void AddImagesToHealthBar(List<Image> bar, Transform barType, int maxHealthValue)
    {
        foreach (Image healthBar in bar)
        {
            Destroy(healthBar.gameObject);
        }
        bar.Clear();

        //Adds additional health bars to playerhealthBarContainer.
        for (int i = 0; i < maxHealthValue; i++)
        {
            Image healthBar = Instantiate(healthBarPrefab, barType);
            bar.Add(healthBar);
        }
    }

    #endregion
}
