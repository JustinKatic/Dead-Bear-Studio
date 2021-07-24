using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIRespawn : MonoBehaviourPun
{
    public AISpawner mySpawnAreaManager;
    public GameObject body;

    public float respawnTimer;


    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {

            yield return new WaitForSeconds(respawnTimer);
            body.transform.position = mySpawnAreaManager.RandomPoint(mySpawnAreaManager.transform.position, mySpawnAreaManager.RadiusCheck);
            body.SetActive(true);

        }
    }
}
