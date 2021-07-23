using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealthManager : HealthManager
{
    private PlayerController player;
    private void Awake()
    {
        player = GetComponent<PlayerController>();
        CurrentHealth = MaxHealth;
        SetStatusStartValues();
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
    void OnDevour()
    {
        if (!photonView.IsMine)
            return;

        StartCoroutine(DevourCorutine());
        IEnumerator DevourCorutine()
        {
            //photonView.RPC("UpdateOverheadText", RpcTarget.All, GameManager.instance.GetPlayer(player.id).photonPlayer.NickName + " Is being devoured");
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
            statusBar.enabled = false;

            Renderer[] renderer = GetComponentsInChildren<Renderer>();
            foreach (Renderer mesh in renderer)
                mesh.enabled = false;

            if (photonView.IsMine)
            {
                int randSpawn = Random.Range(0, GameManager.instance.spawnPoints.Length);
                transform.position = GameManager.instance.spawnPoints[randSpawn].position;
            }


            yield return new WaitForSeconds(3);

            if (photonView.IsMine)
            {
                player.EnableMovement();
                CurrentHealth = MaxHealth;
                photonView.RPC("ChangeStatusBarHealth", RpcTarget.All);
            }
            statusBar.enabled = true;

            foreach (Renderer mesh in renderer)
                mesh.enabled = true;

            canBeDevoured = false;
            beingDevoured = false;
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
            photonView.RPC("ChangeStatusBarStun", RpcTarget.All);
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
                photonView.RPC("ChangeStatusBarHealth", RpcTarget.All);

            }
        }
    }


    protected override void InterruptedDevour()
    {
        base.InterruptedDevour();
    }
}
