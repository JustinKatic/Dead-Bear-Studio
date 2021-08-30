using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MinionHealthManager : HealthManager
{
    [Header("Model To Disable On Death")]
    [SerializeField] GameObject myModel;

    [HideInInspector] public GameObject PlayerWhoShotMe;
    [HideInInspector] public State state;

    private GameObject[] RespawnPositions;
    private Collider col;
    private Canvas hudCanvas;
    private NavMeshAgent agent;

    #region StartUp
    void Awake()
    {
        col = GetComponent<Collider>();
        hudCanvas = GetComponentInChildren<Canvas>();
        RespawnPositions = GameObject.FindGameObjectsWithTag("AIRespawn");
        CurrentHealth = MaxHealth;
        SetAIHealthValues(MaxHealth);
        agent = GetComponent<NavMeshAgent>();
    }
    #endregion

    #region Take Damage/ Heal Damage
    public void TakeDamage(int damage, int attackerID)
    {
        photonView.RPC("TakeDamage_RPC", RpcTarget.All, damage, attackerID);
    }

    [PunRPC]
    public void TakeDamage_RPC(int damage, int attackerID)
    {
        if (photonView.IsMine)
        {
            //Return if already being devoured
            if (beingDevoured)
                return;

            if (CurrentHealth <= 0)
                return;

            //Remove health
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
            //Reset health regen timer
            healthRegenTimer = TimeBeforeHealthRegen;

            //Updates this charcters status bar on all players in network
            UpdateHealthBar(CurrentHealth);

            PlayerWhoShotMe = GameManager.instance.GetPlayer(attackerID).gameObject;

            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                OnBeingStunnedStart();
        }
    }
    protected override void Heal(int amountToHeal)
    {
        //Only running on local player
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        //Updates this charcters health bars on all players in network
        UpdateHealthBar(CurrentHealth);
        healthRegenTimer = TimeBeforeHealthRegen;
    }

    #endregion

    #region Devour
    protected override void OnBeingDevourStart()
    {
        canBeDevoured = false;

        if (photonView.IsMine)
        {
            beingDevoured = true;
        }
    }

    protected override void OnBeingDevourEnd(int attackerID)
    {
        Respawn();
    }
    #endregion

    #region Respawn
    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            myModel.SetActive(false);
            col.enabled = false;
            hudCanvas.enabled = false;
            isStunned = false;
            beingDevoured = false;
            canBeDevoured = false;
            stunnedTimer = 0;

            if (PhotonNetwork.IsMasterClient)
            {
                Stun(false);
                agent.Warp(RespawnPositions[Random.Range(0, RespawnPositions.Length)].transform.position);
            }

            yield return new WaitForSeconds(RespawnTime);

            if (PhotonNetwork.IsMasterClient)
            {
                CurrentHealth = MaxHealth;
                UpdateHealthBar(CurrentHealth);
            }
            myModel.SetActive(true);
            col.enabled = true;
            hudCanvas.enabled = true;
        }
    }
    #endregion

    #region Stun
    void Stun(bool start)
    {
        photonView.RPC("Stun_RPC", RpcTarget.All, start);
    }

    [PunRPC]
    void Stun_RPC(bool start)
    {
        if (start)
        {
            StunVFX.SetActive(true);
            canBeDevoured = true;
        }
        else
        {
            StunVFX.SetActive(false);
            canBeDevoured = false;
        }
    }

    protected override void OnBeingStunnedStart()
    {
        //Things that only affect local
        if (photonView.IsMine)
        {
            Stun(true);
            isStunned = true;
        }
    }

    protected override void OnBeingStunnedEnd()
    {
        if (!beingDevoured)
        {
            //Things that only affect local
            if (photonView.IsMine)
            {
                Stun(false);

                isStunned = false;
                Heal(1);
            }
        }
    }

    #endregion

    #region HealthBar
    void SetAIHealthValues(int MaxHealthValue)
    {
        photonView.RPC("SetAIHealth_RPC", RpcTarget.All, MaxHealthValue);
    }

    [PunRPC]
    protected void SetAIHealth_RPC(int MaxHealthValue)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;
        //Function in Health Manager
        AddImagesToHealthBar(healthBarsOverhead, HealthBarContainerOverhead, MaxHealthValue);

        UpdateHealthBar(CurrentHealth);
    }

    void UpdateHealthBar(int CurrentHealth)
    {
        photonView.RPC("UpdateHealthBar_RPC", RpcTarget.All, CurrentHealth);
    }

    [PunRPC]
    public void UpdateHealthBar_RPC(int CurrentHealth)
    {
        //Function in Health Manager
        FillBarsOfHealth(CurrentHealth, healthBarsOverhead);
    }
    #endregion
}
