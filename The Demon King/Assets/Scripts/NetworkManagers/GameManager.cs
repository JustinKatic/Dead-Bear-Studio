using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;



public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public string spectatorPrefabLocation;

    public SpawnPointRuntimeSet spawnPoints;

    private int playersInGame;

    private int mySpawnIndex;

    public GameObject LoadingScreen;
    public Slider loadingBar;


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
            spectatorPrefab.GetComponent<SpectatorCamera>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
        else
        {
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints.GetItemIndex(mySpawnIndex).transform.position, spawnPoints.GetItemIndex(mySpawnIndex).transform.rotation);
            // initialize the player for all other players
            playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer, spawnPoints.GetItemIndex(mySpawnIndex).transform.eulerAngles.y, spawnPoints.GetItemIndex(mySpawnIndex).transform.eulerAngles.z);
        }
        LoadingScreen.SetActive(false);
    }

    void OnGameStart()
    {
        
    }
}