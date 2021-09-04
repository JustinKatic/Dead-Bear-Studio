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

    [SerializeField] Transform[] RespawnPositions;
    private Collider col;
    private Canvas hudCanvas;
    private NavMeshAgent agent;

    public Image overheadHudHealthbarImg;
    private Material OverheadHealthBarMat;

    public bool Respawned = false;

    #region StartUp
    void Awake()
    {
        OverheadHealthBarMat = Instantiate(overheadHudHealthbarImg.material);
        overheadHudHealthbarImg.material = OverheadHealthBarMat;

        col = GetComponent<Collider>();
        hudCanvas = GetComponentInChildren<Canvas>();
        CurrentHealth = MaxHealth;
        SetAIHealthValues(MaxHealth);
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void Update()
    {
        base.Update();

        if (currentHealthOffset > 0)
        {
            currentHealthOffset -= healthOffsetTime * Time.deltaTime;
            OverheadHealthBarMat.SetFloat("_OffsetHealth", currentHealthOffset);
        }
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
            if (beingDevoured || CurrentHealth <= 0)
                return;

            if (CurrentHealth - damage <= 0)
                currentHealthOffset = damage - (CurrentHealth - damage) * -1;
            else
                currentHealthOffset = damage;


            //Remove health
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, MaxHealth);
            //Reset health regen timer
            healthRegenTimer = 0;

            //Updates this charcters status bar on all players in network
            UpdateHealthBar(CurrentHealth, currentHealthOffset);

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
        UpdateHealthBar(CurrentHealth, 0);
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
                agent.Warp(RespawnPositions[Random.Range(0, RespawnPositions.Length)].position);
                Respawned = true;
            }

            yield return new WaitForSeconds(RespawnTime);

            if (PhotonNetwork.IsMasterClient)
            {
                CurrentHealth = MaxHealth;
                UpdateHealthBar(CurrentHealth, 0);
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
        photonView.RPC("SetHealth_RPC", RpcTarget.All, MaxHealthValue);
    }

    [PunRPC]
    protected void SetHealth_RPC(int MaxHealthValue)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;
        OverheadHealthBarMat.SetFloat("_MaxHealth", MaxHealth);
        UpdateHealthBar(CurrentHealth, 0);
    }

    void UpdateHealthBar(int CurrentHealth, float healthOffset)
    {
        photonView.RPC("UpdateHealthBar_RPC", RpcTarget.All, CurrentHealth, healthOffset);
    }

    [PunRPC]
    public void UpdateHealthBar_RPC(int CurrentHealth, float healthOffset)
    {
        OverheadHealthBarMat.SetFloat("_CurrentHealth", CurrentHealth);
        if (!photonView.IsMine)
            currentHealthOffset += healthOffset;

    }
    #endregion
}
