﻿using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPun
{
    [Header("Players")]
    public string playerPrefabLocation;
    public List<PlayerController> players;
    public Transform[] spawnPoints;

    private int playersInGame;

    public int myIdIndex;
    private bool indexAssigned = false;

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
        players = new List<PlayerController>();
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            players.Add(null);
        }

        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        indexAssigned = false;
        while (!indexAssigned)
        {
            if (PhotonNetwork.LocalPlayer.GetPlayerNumber() != -1)
            {
                myIdIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();
                indexAssigned = true;
                Debug.Log(myIdIndex);
                Debug.Log("Im in game");
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
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[myIdIndex].position, Quaternion.identity);

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