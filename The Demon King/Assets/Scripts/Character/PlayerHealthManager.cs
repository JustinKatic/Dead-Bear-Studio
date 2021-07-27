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

    public Canvas playerOverheadHealthBar;
    public Transform playerHealthBarContainerOverhead;
    private List<Image> healthBarsOverhead = new List<Image>();


    public Canvas playerHealthBar;
    public Transform playerHealthBarContainer;
    public Image healthBarPrefab;
    private List<Image> healthBars = new List<Image>();

    private IEnumerator myDevourCo;


    void Awake()
    {
        //Run following if not local player
        if (!photonView.IsMine)
        {
            Destroy(playerHealthBar.gameObject);
        }
        //Run following if local player
        else
        {
            Destroy(playerOverheadHealthBar.gameObject);
            CurrentHealth = MaxHealth;
            player = GetComponent<PlayerController>();

            photonView.RPC("SetHealth", RpcTarget.All, MaxHealth, CurrentHealth);
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
    void SetHealth(int MaxHealthValue, int CurrentHealthValue)
    {
        //Run following on everyone
        MaxHealth = MaxHealthValue;

        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;


        //Run following if not local player
        if (!photonView.IsMine)
        {
            if (MaxHealth > MaxHealthValue)
            {
                foreach (Image healthBar in healthBarsOverhead)
                {
                    Destroy(healthBar.gameObject);
                }
                healthBarsOverhead.Clear();
            }
            //Adds additional health bars to playerhealthBarContainer.
            for (int i = healthBarsOverhead.Count; i < MaxHealthValue; i++)
            {
                Image healthBar = Instantiate(healthBarPrefab, playerHealthBarContainerOverhead);
                healthBarsOverhead.Add(healthBar);
            }
        }
        //Run following if local player
        else
        {
            if (MaxHealth > MaxHealthValue)
            {
                foreach (Image healthBar in healthBars)
                {
                    Destroy(healthBar.gameObject);
                }
                healthBars.Clear();
            }

            //Adds additional health bars to playerhealthBarContainer.
            for (int i = healthBars.Count; i < MaxHealthValue; i++)
            {
                Image healthBar = Instantiate(healthBarPrefab, playerHealthBarContainer);
                healthBars.Add(healthBar);
            }

            photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
        }
    }


    protected override void Heal(int amountToHeal)
    {
        //Only running on local player
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        //Updates this charcters health bars on all players in network
        photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
        HealthRegenTimer = TimeBeforeHealthRegen;
    }


    [PunRPC]
    void OnDevour()
    {
        if (!photonView.IsMine)
            return;

        myDevourCo = DevourCorutine();
        StartCoroutine(myDevourCo);

        IEnumerator DevourCorutine()
        {
            beingDevoured = true;

            yield return new WaitForSeconds(DevourTime);

            photonView.RPC("Respawn", RpcTarget.All);
        }
    }

    [PunRPC]
    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            Renderer[] renderer = GetComponentsInChildren<Renderer>();
            foreach (Renderer mesh in renderer)
                mesh.enabled = false;



            if (photonView.IsMine)
            {
                int randSpawn = Random.Range(0, GameManager.instance.spawnPoints.Length);
                player.DisableMovement();
                transform.position = GameManager.instance.spawnPoints[randSpawn].position;
            }

            yield return new WaitForSeconds(3);

            if (photonView.IsMine)
            {
                player.EnableMovement();
                CurrentHealth = MaxHealth;
                photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);
            }
            foreach (Renderer mesh in renderer)
                mesh.enabled = true;

            canBeDevoured = false;
            beingDevoured = false;
            isStunned = false;
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

            if (CurrentHealth <= 0)
                return;
            //Remove health
            CurrentHealth -= damage;

            photonView.RPC("UpdateHealthBar", RpcTarget.All, CurrentHealth);

            //Id of who attacked us
            curAttackerId = attackerId;

            //Reset health regen timer
            HealthRegenTimer = TimeBeforeHealthRegen;


            //call Stunned() on all player on network if no health left
            if (CurrentHealth <= 0)
                photonView.RPC("Stunned", RpcTarget.All);
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
            Debug.Log("Play Stunned Anim");
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
                isStunned = false;
                player.EnableMovement();
                Heal(1);
                Debug.Log("Stop Stunned Anim");
            }
        }
    }

    [PunRPC]
    protected override void InterruptDevourOnSelf()
    {
        beingDevoured = false;
        StopCoroutine(myDevourCo);
    }
}
