using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayers = 10;

    public string RoomName;
    
    // instance
    public static NetworkManager instance;
    void Awake ()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start ()
    {
        // connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster ()
    {
        PhotonNetwork.JoinLobby();
    }

    // creates a new room of the requested room name
    public void CreateRoom (string roomName, int roomMaxPlayers, bool publicRoom)
    {
        RoomOptions options = new RoomOptions();
        //maxPlayers = roomMaxPlayers;
        options.MaxPlayers = (byte)roomMaxPlayers;
        
        RoomName = roomName.ToUpper();
        options.IsVisible = publicRoom;

        PhotonNetwork.CreateRoom(RoomName, options);
    }
    
    // joins a room of the requested room name
    public void JoinRoom (string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    // changes the scene through Photon's system
    [PunRPC]
    public void ChangeScene (string sceneName)
    {
        //Checks if the scene is found within the build settings, otherwise load game as default
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {
            PhotonNetwork.LoadLevel("Game");
            Debug.Log("Scene Not Found in Build Settings");
        }
    }

    // called when we disconnect from the Photon server
    public override void OnDisconnected (DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnPlayerLeftRoom (Player otherPlayer)
    {
   
    }
}