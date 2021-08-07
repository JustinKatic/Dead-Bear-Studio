using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class MinionHealthManager : HealthManager
{
    public GameObject model;
    public float respawnTimer;
    private GameObject[] RespawnPositions;
    private Collider col;
    private Canvas hudCanvas;
    public GameObject PlayerWhoShotMe;
    public State state;
    private NavMeshAgent agent;

    void Awake()
    {
        col = GetComponent<Collider>();
        hudCanvas = GetComponentInChildren<Canvas>();
        RespawnPositions = GameObject.FindGameObjectsWithTag("AIRespawn");
        CurrentHealth = MaxHealth;
        photonView.RPC("SetAIHealth", RpcTarget.All, MaxHealth);
        agent = GetComponent<NavMeshAgent>();
    }


    [PunRPC]
    public void TakeDamage(int damage, int attackerID)
    {
        if (photonView.IsMine)
        {
            //Return if already being devoured
            if (beingDevoured)
                return;

            if (CurrentHealth <= 0)
                return;

            //Remove health
            CurrentHealth -= damage;
            //Reset health regen timer
            HealthRegenTimer = TimeBeforeHealthRegen;

            //Updates this charcters status bar on all players in network
            photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);

            PlayerWhoShotMe = GameManager.instance.GetPlayer(attackerID).gameObject;

            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                photonView.RPC("Stunned", RpcTarget.All);
        }
    }

    [PunRPC]
    void OnDevour()
    {
        myDevourCo = DevourCorutine();
        StartCoroutine(myDevourCo);


        IEnumerator DevourCorutine()
        {
            canBeDevoured = false;

            if (photonView.IsMine)
            {
                beingDevoured = true;
            }

            yield return new WaitForSeconds(DevourTime);

            Respawn();
        }
    }

    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            model.SetActive(false);
            col.enabled = false;
            hudCanvas.enabled = false;



            if (PhotonNetwork.IsMasterClient)
            {
                agent.Warp(RespawnPositions[Random.Range(0, RespawnPositions.Length)].transform.position);
            }

            yield return new WaitForSeconds(respawnTimer);


            if (photonView.IsMine)
            {
                CurrentHealth = MaxHealth;
                photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
                beingDevoured = false;
            }
            model.SetActive(true);
            col.enabled = true;
            hudCanvas.enabled = true;
            canBeDevoured = false;
            isStunned = false;
        }
    }

    protected override void OnStunStart()
    {
        //Things that affect everyone
        canBeDevoured = true;

        //Things that only affect local
        if (photonView.IsMine)
        {
            isStunned = true;
            Debug.Log("Play Stun Anim");
        }
    }

    protected override void OnStunEnd()
    {
        if (!beingDevoured)
        {
            //Things that affect everyone
            canBeDevoured = false;

            //Things that only affect local
            if (photonView.IsMine)
            {
                isStunned = false;
                Heal(1);
                Debug.Log("Stop Playing Stun Anim");
            }
        }
    }

    [PunRPC]
    protected void SetAIHealth(int MaxHealthValue)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;

        foreach (Image healthBar in healthBarsOverhead)
        {
            Destroy(healthBar.gameObject);
        }
        healthBarsOverhead.Clear();

        //Adds additional health bars to playerhealthBarContainer.
        for (int i = healthBarsOverhead.Count; i < MaxHealthValue; i++)
        {
            Image healthBar = Instantiate(healthBarPrefab, HealthBarContainerOverhead);
            healthBarsOverhead.Add(healthBar);
        }
        photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);

    }
    [PunRPC]
    public void UpdateHealthBar(int CurrentHealth)
    {
        for (int i = 0; i < MaxHealth; i++)
        {
            //Change health bar red if the bar we are looking at is < currentHealth
            if (i < CurrentHealth)
                healthBarsOverhead[i].color = Color.red;
            //Change health bar transparent if the bar we are looking at is > currentHealth
            else
                healthBarsOverhead[i].color = new Color(255, 0, 0, 0);
        }
    }
}
