using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIRespawn : MonoBehaviourPun
{
    public AISpawner mySpawnAreaManager;


    public float respawnTimer;

    public GameObject model;
    private Collider col;
    private Canvas hudCanvas;
    private HealthManager healthManager;

    void Awake()
    {
        col = GetComponent<Collider>();
        hudCanvas = GetComponentInChildren<Canvas>();
        healthManager = GetComponent<HealthManager>();
    }


    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            model.SetActive(false);
            col.enabled = false;
            hudCanvas.enabled = false;
            healthManager.canBeDevoured = false;
            

            if (PhotonNetwork.IsMasterClient)
            {
                transform.position = mySpawnAreaManager.RandomPoint(mySpawnAreaManager.transform.position, mySpawnAreaManager.RadiusCheck);
            }

            yield return new WaitForSeconds(respawnTimer);


            model.SetActive(true);
            col.enabled = true;
            hudCanvas.enabled = true;
        }
    }
}
