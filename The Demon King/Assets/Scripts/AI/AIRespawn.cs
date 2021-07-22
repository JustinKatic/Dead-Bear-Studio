using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AIRespawn : MonoBehaviourPun
{
    public AISpawner mySpawnAreaManager;
    public GameObject myBody;

    [PunRPC]
    public void Respawn()
    {
        StartCoroutine(ResetPlayer());

        IEnumerator ResetPlayer()
        {
            if (photonView.IsMine)
            {
                myBody.SetActive(false);
            }


            yield return new WaitForSeconds(3);

            if (photonView.IsMine)
            {
                transform.position = mySpawnAreaManager.RandomPoint(mySpawnAreaManager.transform.position, mySpawnAreaManager.RadiusCheck);
                myBody.SetActive(true);
            }
        }
    }
}
