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
    public bool levelNotLoading = true;

    // instance
    public static NetworkManager instance;

    private PhotonView PV;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            if (PV == null)
            {
                PV = gameObject.AddComponent<PhotonView>();
                photonView.ViewID = 999;
            }
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // connect to the master server
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

    }

    // creates a new room of the requested room name
    public void CreateRoom(string roomName, int roomMaxPlayers)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)roomMaxPlayers;

        RoomName = roomName.ToUpper();

        PhotonNetwork.CreateRoom(RoomName, options);
    }

    // joins a room of the requested room name
    public void JoinRoom(string roomName)
    {
        RoomName = roomName;
        PhotonNetwork.JoinRoom(roomName);
    }

    // changes the scene through Photon's system
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        levelNotLoading = false;
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


        levelNotLoading = true;
    }

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