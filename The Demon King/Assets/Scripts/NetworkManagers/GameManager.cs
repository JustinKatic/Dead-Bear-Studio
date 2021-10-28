using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;


public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public string spectatorPrefabLocation;

    public SpawnPointRuntimeSet spawnPoints;

    private int playersInGame;
    private float waitToStartGameTimer = 5f;

    private int mySpawnIndex;
    public GameObject LoadingScreen;
    public Slider loadingBar;
    private List<PlayerController> playerObjList = new List<PlayerController>();
    private List<SpectatorCamera> spectatorObjList = new List<SpectatorCamera>();
    
    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
            SceneManager.LoadScene("Menu");

        loadingBar.maxValue = PhotonNetwork.PlayerList.Length;
        loadingBar.value = playersInGame;
    }


    void Start()
    {
        StartCoroutine(AssignSpawnIndex());
    }

    IEnumerator AssignSpawnIndex()
    {
        bool indexAssigned = false;
        while (!indexAssigned)
        {
            if (PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1)
            {
                mySpawnIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
                indexAssigned = true;
                ImInGame();
            }
            yield return null;
        }
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
            SpawnPlayers();
            
            if (playerObjList != null)
            {
                foreach (var playerController in playerObjList)
                {
                    playerController.DisableMovement();
                }
            }
            
            StartCoroutine(WaitForGameStart());
        }
    }

    public void SpawnPlayers()
    {
        photonView.RPC("SpawnPlayer_RPC", RpcTarget.All);
    }
    
    [PunRPC]
    void SpawnPlayer_RPC()
    {
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsSpectator"] == true)
        {
            //LOAD SPECTAOR PREFAB
            GameObject spectatorPrefab = PhotonNetwork.Instantiate(spectatorPrefabLocation, spawnPoints.GetItemIndex(mySpawnIndex).transform.position, spawnPoints.GetItemIndex(mySpawnIndex).transform.rotation);
            SpectatorCamera spectatorCamera = spectatorPrefab.GetComponent<SpectatorCamera>();
            spectatorObjList.Add(spectatorCamera);
            
            spectatorCamera.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

        }
        else
        {
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints.GetItemIndex(mySpawnIndex).transform.position, spawnPoints.GetItemIndex(mySpawnIndex).transform.rotation);

            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerObjList.Add(playerController);
            // initialize the player for all other players
            playerController.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer, spawnPoints.GetItemIndex(mySpawnIndex).transform.eulerAngles.y, spawnPoints.GetItemIndex(mySpawnIndex).transform.eulerAngles.z);
        }
        LoadingScreen.SetActive(false);
    }

    void StartGame()
    {
        if (playerObjList != null)
        {
            foreach (var playerController in playerObjList)
            {
                playerController.EnableMovement();
            }
        }
    }

    IEnumerator WaitForGameStart()
    {
        yield return new WaitForSeconds(waitToStartGameTimer);
        
        StartGame();
    }
   
}