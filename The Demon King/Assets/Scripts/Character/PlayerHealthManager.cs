using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealthManager : HealthManager
{
    private PlayerController player;


    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerController>();
    }


    [PunRPC]
    void OnDevour()
    {
        if (!photonView.IsMine)
            return;

        StartCoroutine(DevourCorutine());
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
            statusBar.enabled = false;

            Renderer[] renderer = GetComponentsInChildren<Renderer>();
            foreach (Renderer mesh in renderer)
                mesh.enabled = false;

            if (photonView.IsMine)
            {
                int randSpawn = Random.Range(0, GameManager.instance.spawnPoints.Length);

                player.DisableMovement();
                transform.position = GameManager.instance.spawnPoints[randSpawn].position;
                player.EnableMovement();
            }

            yield return new WaitForSeconds(3);

            if (photonView.IsMine)
            {
                player.EnableMovement();
                CurrentHealth = MaxHealth;
                photonView.RPC("UpdateStatusBar", RpcTarget.All, CurrentHealth);
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


    protected override void InterruptedDevour()
    {
        base.InterruptedDevour();
    }
}
