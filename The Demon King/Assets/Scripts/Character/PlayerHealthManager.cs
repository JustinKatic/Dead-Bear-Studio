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
    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            OverheadText.enabled = false;

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
                photonView.RPC("UpdateOverheadText", RpcTarget.All, CurrentHealth.ToString());
            }
            OverheadText.enabled = true;

            foreach (Renderer mesh in renderer)
                mesh.enabled = true;

            canBeDevoured = false;
            beingDevoured = false;
            isStunned = false;
        }
    }

    protected override void OnStunEnd()
    {
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

    protected override void OnStunStart()
    {
        //Things that only affect local
        if (photonView.IsMine)
        {
            player.DisableMovement();
        }
    }

    protected override void InterruptedDevour()
    {
        base.InterruptedDevour();
    }
}
