using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerHealthManager : HealthManager
{
    private PlayerController player;
    private ExperienceManager experienceManager;
    public int experienceLoss = 2;
    public GameObject StunVFX;



    void Awake()
    {
        //Run following if not local player
        if (!photonView.IsMine)
        {
            Destroy(HealthBar.gameObject);
        }
        //Run following if local player
        else
        {
            Destroy(overheadHealthBar.gameObject);
            CurrentHealth = MaxHealth;
            player = GetComponent<PlayerController>();
            experienceManager = GetComponent<ExperienceManager>();
            photonView.RPC("SetHealth", RpcTarget.All, MaxHealth);
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

            if (photonView.IsMine)
            {
                photonView.RPC("Respawn", RpcTarget.All);
            }
        }
    }


    [PunRPC]
    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            Evolutions currentActiveEvolution = gameObject.GetComponentInChildren<Evolutions>();
            currentActiveEvolution?.gameObject.SetActive(false);

            if (photonView.IsMine)
            {
                photonView.RPC("StunRPC", RpcTarget.All, false);
                GameManager.instance.photonView.RPC("IncrementSpawnPos", RpcTarget.All);
                player.DisableMovement();
                player.cc.enabled = false;
                transform.position = GameManager.instance.spawnPoints[GameManager.instance.spawnIndex].position;
                player.cc.enabled = true;
                player.currentAnim.SetBool("Stunned", false);
            }
            else
            {
                overheadHealthBar.enabled = false;
            }

            yield return new WaitForSeconds(3);

            if (photonView.IsMine)
            {
                player.EnableMovement();
                CurrentHealth = MaxHealth;
                photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
                experienceManager.CheckEvolutionOnDeath(MyMinionType, experienceLoss);
            }
            else
            {
                overheadHealthBar.enabled = true;
            }
            if (gameObject.GetComponentInChildren<Evolutions>() == null)
                currentActiveEvolution?.gameObject.SetActive(true);

            canBeDevoured = false;
            beingDevoured = false;
            isStunned = false;
        }
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        //Runing following if local player
        if (photonView.IsMine)
        {
            //Return if already being devoured
            if (beingDevoured)
                return;

            if (CurrentHealth <= 0)
                return;
            //Remove health
            CurrentHealth -= damage;

            photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);

            //Reset health regen timer
            HealthRegenTimer = TimeBeforeHealthRegen;


            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                photonView.RPC("Stunned", RpcTarget.All);
        }
    }
    [PunRPC]
    protected void SetHealth(int MaxHealthValue)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;

        //Run following if not local player
        if (!photonView.IsMine)
        {
            foreach (Image healthBar in healthBarsOverhead)
            {
                Destroy(healthBar.gameObject);
            }
            healthBarsOverhead.Clear();

            //Adds additional health bars to playerhealthBarContainer.
            for (int i = 0; i < MaxHealthValue; i++)
            {
                Image healthBar = Instantiate(healthBarPrefab, HealthBarContainerOverhead);
                healthBarsOverhead.Add(healthBar);
            }
        }
        //Run following if local player
        else
        {
            foreach (Image healthBar in healthBars)
            {
                Destroy(healthBar.gameObject);
            }
            healthBars.Clear();


            //Adds additional health bars to playerhealthBarContainer.
            for (int i = 0; i < MaxHealthValue; i++)
            {
                Image healthBar = Instantiate(healthBarPrefab, HealthBarContainer);
                healthBars.Add(healthBar);
            }

            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;

            photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
        }
    }

    [PunRPC]
    public void UpdateHealthBar(int CurrentHealth)
    {
        //Run following if local player
        if (photonView.IsMine)
        {
            for (int i = 0; i < MaxHealth; i++)
            {
                //Change health bar red if the bar we are looking at is < currentHealth
                if (i < CurrentHealth)
                    healthBars[i].color = Color.red;
                //Change health bar transparent if the bar we are looking at is > currentHealth
                else
                    healthBars[i].color = new Color(255, 0, 0, 0);
            }
        }
        else
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

    [PunRPC]
    void StunRPC(bool start)
    {
        if (start)
            StunVFX.SetActive(true);
        else
            StunVFX.SetActive(false);
    }

    protected override void OnStunStart()
    {
        //Things that affect everyone
        canBeDevoured = true;

        //Things that only affect local
        if (photonView.IsMine)
        {
            photonView.RPC("StunRPC", RpcTarget.All, true);
            isStunned = true;
            player.currentAnim.SetBool("Devouring", false);
            player.currentAnim.SetBool("Stunned", true);
            player.DisableMovement();
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
                photonView.RPC("StunRPC", RpcTarget.All, false);
                isStunned = false;
                player.EnableMovement();
                Heal(1);
                player.currentAnim.SetBool("Stunned", false);
            }
        }
    }



}
