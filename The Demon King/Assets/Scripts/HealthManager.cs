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
    [Tooltip("Max health value")]
    public int MaxHealth = 3;
    [Tooltip("Time before health regen, Reset on dmg taken")]
    public float HealthRegenTimer = 3f;

    [Header("Stun/Devour Duration")]
    [Tooltip("Time spent being devoured")]
    public float DevourTime = 3f;
    public float stunnedDuration;


    protected float TimeBeforeHealthRegen = 3f;
    protected int curAttackerId;

    [HideInInspector] public Slider statusBar = null;

    [HideInInspector] public int CurrentHealth = 0;
    [HideInInspector] public bool beingDevoured = false;
    [HideInInspector] public bool canBeDevoured = false;
    [HideInInspector] public bool isStunned = false;


    protected virtual void Awake()
    {
        //Get value of everyones status bar and set there sliders max length and values so accurate when adjust later in rpc calls
        statusBar = GetComponentInChildren<Slider>();
        CurrentHealth = MaxHealth;
        statusBar.maxValue = MaxHealth;
        statusBar.value = CurrentHealth;
    }

    private void Update()
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            //Heal every X seconds if not at max health
            if (CurrentHealth < MaxHealth)
            {
                HealthRegenTimer -= Time.deltaTime;
                if (HealthRegenTimer <= 0)
                    Heal(1);
            }
        }
    }


    //Updates the players status to everyone on the server
    [PunRPC]
    public void UpdateStatusBar(int value)
    {
        statusBar.value = value;
    }


    //Runing following if local player
    public void Heal(int amountToHeal)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        //Updates this charcters status bar on all players in network
        photonView.RPC("UpdateStatusBar", RpcTarget.All, CurrentHealth);
        statusBar.value = CurrentHealth;
        HealthRegenTimer = TimeBeforeHealthRegen;
    }


    //This is run when the player has been stunned
    [PunRPC]
    protected void Stunned()
    {
        if (!isStunned)
            StartCoroutine(StunnedCorutine());

        IEnumerator StunnedCorutine()
        {
            OnStunStart();

            yield return new WaitForSeconds(stunnedDuration);

            OnStunEnd();
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        //Runing following if local player
        if (photonView.IsMine)
        {
            //Return if already being devoured
            if (beingDevoured)
                return;

            //Remove health
            CurrentHealth -= damage;

            //Id of who attacked us
            curAttackerId = attackerId;

            //Reset health regen timer
            HealthRegenTimer = TimeBeforeHealthRegen;

            //Updates this charcters status bar on all players in network
            photonView.RPC("UpdateStatusBar", RpcTarget.All, CurrentHealth);

            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                photonView.RPC("Stunned", RpcTarget.All);
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (photonView.IsMine)
        {
            //Return if already being devoured
            if (beingDevoured)
                return;

            //Remove health
            CurrentHealth -= damage;
            //Reset health regen timer
            HealthRegenTimer = TimeBeforeHealthRegen;

            //Updates this charcters status bar on all players in network
            photonView.RPC("UpdateStatusBar", RpcTarget.All, CurrentHealth);

            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                photonView.RPC("Stunned", RpcTarget.All);
        }
    }


    //Overrides for inherited classes
    protected virtual void OnStunStart()
    {

    }

    protected virtual void InterruptedDevour()
    {

    }

    protected virtual void OnStunEnd()
    {

    }

}
