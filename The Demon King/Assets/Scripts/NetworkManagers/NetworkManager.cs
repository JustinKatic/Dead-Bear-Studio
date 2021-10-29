using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // called when we disconnect from the Photon server
    public PlayerControllerRuntimeSet playerControllerRuntimeSet;

    private const byte DisplayPlayerLeftLobby = 6;


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

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                SendOptions sendOptions = new SendOptions { Reliability = true };
                object[] data = new object[] { otherPlayer.NickName };
                PhotonNetwork.RaiseEvent(DisplayPlayerLeftLobby, data, raiseEventOptions, sendOptions);

                if (!IsThereAKing)
                    GameObject.FindGameObjectWithTag("DemonKingCrown").GetComponent<CrownHealthManager>().CrownRespawn(true);
            }
        }
    }
}