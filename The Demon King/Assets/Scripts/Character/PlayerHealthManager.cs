using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealthManager : MonoBehaviourPun
{
    [Header("HealthStats")]
    public int MaxHealth = 3;
    public int CurrentHealth = 0;
    public float HealthRegenTimer = 3f;
    private float TimeBeforeHealthRegen = 3f;

    [Header("StunStats")]
    public float stunnedDuration;

    public bool Dead = false;
    private int curAttackerId;

    public TMP_Text OverheadText = null;
    private PlayerController player;

    public bool beingDevoured = false;
    public bool canBeDevoured = false;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        CurrentHealth = MaxHealth;
        OverheadText.text = CurrentHealth.ToString();
    }

    private void Update()
    {
        if (CurrentHealth < MaxHealth)
        {
            HealthRegenTimer -= Time.deltaTime;
            if (HealthRegenTimer <= 0)
            {
                photonView.RPC("Heal", player.photonPlayer, 1);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int attackerId, int damage)
    {
        if (beingDevoured)
            return;

        CurrentHealth -= damage;
        curAttackerId = attackerId;

        HealthRegenTimer = TimeBeforeHealthRegen;

        photonView.RPC("UpdateOverheadText", RpcTarget.All, CurrentHealth.ToString());

        //die if no health left
        if (CurrentHealth <= 0)
            photonView.RPC("Stunned", RpcTarget.All);
    }

    //Updates the players text to everyone on the server
    [PunRPC]
    public void UpdateOverheadText(string textToDisplay)
    {
        OverheadText.text = textToDisplay;
    }


    [PunRPC]
    public void Heal(int amountToHeal)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amountToHeal, 0, MaxHealth);
        photonView.RPC("UpdateOverheadText", RpcTarget.All, CurrentHealth.ToString());
        OverheadText.text = CurrentHealth.ToString();
        HealthRegenTimer = TimeBeforeHealthRegen;
    }

    //This is run when the player has been stunned
    [PunRPC]
    void Stunned()
    {
        StartCoroutine(StunnedCorutine());

        IEnumerator StunnedCorutine()
        {
            //Things that only affect local
            if (photonView.IsMine)
            {
                player.DisableMovement();
            }
            //Things that affect everyone
            canBeDevoured = true;
            photonView.RPC("UpdateOverheadText", RpcTarget.All, "Stunned");

            yield return new WaitForSeconds(stunnedDuration);

            if (!beingDevoured)
            {
                //Things that only affect local
                if (photonView.IsMine)
                {
                    player.EnableMovement();
                }
                //Things that affect everyone
                canBeDevoured = false;
            }
        }
    }


    [PunRPC]
    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            if (photonView.IsMine)
            {
                int randSpawn = Random.Range(0, GameManager.instance.spawnPoints.Length);
                transform.position = GameManager.instance.spawnPoints[randSpawn].position;
            }
            OverheadText.enabled = false;
            Renderer[] renderer = GetComponentsInChildren<Renderer>();
            foreach (Renderer mesh in renderer)
                mesh.enabled = false;


            yield return new WaitForSeconds(3);

            if (photonView.IsMine)
            {
                player.EnableMovement();
                CurrentHealth = MaxHealth;
            }
            foreach (Renderer mesh in renderer)
                mesh.enabled = true;
            OverheadText.enabled = true;
            canBeDevoured = false;
            beingDevoured = false;
            photonView.RPC("UpdateOverheadText", RpcTarget.All, CurrentHealth.ToString());
        }
    }
}
