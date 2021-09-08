using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class  GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerController[] players;
    public Transform[] spawnPoints;

    private int playersInGame;

    public int spawnIndex = 0;

    // instance
    public static GameManager instance;

    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
            SceneManager.LoadScene("Menu");

        instance = this;
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];

        ImInGame();
    }

    void ImInGame()
    {
        photonView.RPC("ImInGame_RPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ImInGame_RPC()
    {
        playersInGame++;

        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        photonView.RPC("SpawnPlayer_RPC", RpcTarget.All);
    }

    [PunRPC]
    void SpawnPlayer_RPC()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);

        // initialize the player for all other players
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }


    public void IncrementSpawnPos()
    {
        photonView.RPC("IncrementSpawnPos_RPC", RpcTarget.All);
    }


    [PunRPC]
    public void IncrementSpawnPos_RPC()
    {
        spawnIndex++;

        if (spawnIndex >= spawnPoints.Length)
            spawnIndex = 0;
    }


    public PlayerController GetPlayer(int playerId)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.id == playerId)
                return player;
        }

        return null;
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        foreach (PlayerController player in players)
        {
            if (player != null && player.gameObject == playerObject)
                return player;
        }

        return null;
    }
}