using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // called when we disconnect from the Photon server
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("A Player left room");
        if (PhotonNetwork.IsMasterClient && GameManager.instance != null)
        {
            bool IsThereAKing = false;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (GameManager.instance.GetPlayer((int)player.CustomProperties["PlayerId"]).GetComponent<DemonKingEvolution>().AmITheDemonKing)
                {
                    IsThereAKing = true;
                }

            }
            if (!IsThereAKing)
                GameObject.FindGameObjectWithTag("DemonKingCrown").GetComponent<CrownHealthManager>().CrownRespawn(true);
        }
    }
}