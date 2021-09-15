using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerController[] players;
    public Transform[] spawnPoints;

    private int playersInGame;

    private int mySpawnIndex;

    public int spawnIndex = 0;

    public GameObject LoadingScreen;
    public Slider loadingBar;

    // instance
    public static GameManager instance;


    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
            SceneManager.LoadScene("Menu");

        loadingBar.maxValue = PhotonNetwork.PlayerList.Length;
        loadingBar.value = playersInGame;

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
        loadingBar.value = playersInGame;

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
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[PlayerNumberingExtensions.GetPlayerNumber(PhotonNetwork.LocalPlayer)].position, Quaternion.identity);

        // initialize the player for all other players
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

        LoadingScreen.SetActive(false);
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