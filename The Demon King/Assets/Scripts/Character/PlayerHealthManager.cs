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
    private List<Image> healthaBars = new List<Image>();

    private IEnumerator myDevourCo;


    protected override void Awake()
    {
        if (photonView.IsMine)
            statusBar.gameObject.SetActive(false);        
        else
        {
            base.Awake();

            Destroy(playerHealthBar.gameObject);
        }

        healthaBars = playerHealthBar.transform.GetChild(0).GetComponentsInChildren<Image>().ToList();
        
        player = GetComponent<PlayerController>();
        
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
                //photonView.RPC("UpdateStatusBar", RpcTarget.All, CurrentHealth);
                MyHealthBarUIUpdate();
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

    [PunRPC]
    protected override void InterruptDevourOnSelf()
    {
        beingDevoured = false;
        StopCoroutine(myDevourCo);
    }

    void MyHealthBarUIUpdate()
    {
        for (int i = 0; i < healthaBars.Count; i++)
        {
            if (i <= CurrentHealth)
            {
                //healthaBars[i].material.
            }
            
        }
        
    }
}
