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
    public Canvas playerHealthBar;
    public Transform playerHealthBarContainer;
    public Image healthBarPrefab;
    private List<Image> healthBars = new List<Image>();

    private IEnumerator myDevourCo;


    protected override void Awake()
    {
        base.Awake();

        if (!photonView.IsMine)
        {
            Destroy(playerHealthBar.gameObject);
        }
        else
        {
            statusBar.gameObject.SetActive(false);
            player = GetComponent<PlayerController>();

            for (int i = 0; i < CurrentHealth; i++)
            {
                Image healthBar = Instantiate(healthBarPrefab, playerHealthBarContainer);
                healthBars.Add(healthBar);
            }
        }
    }

    public void UpdateHealthBar()
    {
        if (photonView.IsMine)
        {
            for (int i = 0; i < MaxHealth; i++)
            {
                if (i < CurrentHealth)
                    healthBars[i].color = Color.red;
                else
                    healthBars[i].color = new Color(255, 0, 0, 0);
            }
        }
    }



    [PunRPC]
    public void ChangeMaxHealth(int NewMaxHealth)
    {
        MaxHealth = NewMaxHealth;

        if (photonView.IsMine)
        {
            for (int i = healthBars.Count; i < NewMaxHealth; i++)
            {
                Image healthBar = Instantiate(healthBarPrefab, playerHealthBarContainer);
                healthBars.Add(healthBar);
            }
        }
    }


    protected override void Heal(int amountToHeal)
    {
        //Only running on local player
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        //Updates this charcters status bar on all players in network
        photonView.RPC("UpdateStatusBar", RpcTarget.Others, CurrentHealth);
        UpdateHealthBar();

        statusBar.value = CurrentHealth;
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
                photonView.RPC("ChangeMaxHealth", RpcTarget.All, 6);
                CurrentHealth = MaxHealth;
                UpdateHealthBar();
                photonView.RPC("UpdateStatusbarValues", RpcTarget.All);
                photonView.RPC("UpdateStatusBar", RpcTarget.Others, CurrentHealth);
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

            //Remove health
            CurrentHealth -= damage;

            UpdateHealthBar();

            //Id of who attacked us
            curAttackerId = attackerId;

            //Reset health regen timer
            HealthRegenTimer = TimeBeforeHealthRegen;

            //Updates this charcters status bar on all players in network
            photonView.RPC("UpdateStatusBar", RpcTarget.Others, CurrentHealth);

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
