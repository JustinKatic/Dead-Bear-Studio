using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    // called when we disconnect from the Photon server
    public PlayerControllerRuntimeSet playerControllerRuntimeSet;

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("A Player left room");
        if (PhotonNetwork.IsMasterClient)
        {
            bool IsThereAKing = false;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (playerControllerRuntimeSet.GetPlayer((int)player.CustomProperties["PlayerId"]).GetComponent<DemonKingEvolution>().AmITheDemonKing)
                {
                    IsThereAKing = true;
                }

            }
            if (!IsThereAKing)
                GameObject.FindGameObjectWithTag("DemonKingCrown").GetComponent<CrownHealthManager>().CrownRespawn(true);
        }
    }
}