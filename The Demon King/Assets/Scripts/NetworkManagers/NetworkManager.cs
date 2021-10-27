using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;


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
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Debug.Log("A Player the lobby room");
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("A Player left the game room");
                bool IsThereAKing = false;
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    if (playerControllerRuntimeSet.GetPlayer(player.ActorNumber).GetComponent<DemonKingEvolution>().AmITheDemonKing)
                    {
                        IsThereAKing = true;
                    }
                }
                if (!IsThereAKing)
                    GameObject.FindGameObjectWithTag("DemonKingCrown").GetComponent<CrownHealthManager>().CrownRespawn(true);
            }
        }
    }
}