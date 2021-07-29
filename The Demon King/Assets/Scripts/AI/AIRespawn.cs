using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIRespawn : MonoBehaviourPun
{
    public AISpawner mySpawnAreaManager;


    public float respawnTimer;

    private MeshRenderer mR;
    private Collider col;
    private Canvas hudCanvas;

    void Awake()
    {
        mR = GetComponent<MeshRenderer>();
        col = GetComponent<Collider>();
        hudCanvas = GetComponentInChildren<Canvas>();
    }


    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            mR.enabled = false;
            col.enabled = false;
            hudCanvas.enabled = false;

            yield return new WaitForSeconds(respawnTimer);

            if (photonView.IsMine)
            {
                transform.position = mySpawnAreaManager.RandomPoint(mySpawnAreaManager.transform.position, mySpawnAreaManager.RadiusCheck);
            }

            mR.enabled = true;
            col.enabled = true;
            hudCanvas.enabled = true;
        }
    }
}
