using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using System.Collections;
using TMPro;



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
    public PlayerControllerRuntimeSet playerControllerRuntimeSet;
    public TextMeshProUGUI countDownText;



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
            SpectatorCamera spectatorCamera = spectatorPrefab.GetComponent<SpectatorCamera>();

            spectatorCamera.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
        else
        {
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints.GetItemIndex(mySpawnIndex).transform.position, spawnPoints.GetItemIndex(mySpawnIndex).transform.rotation);

            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            // initialize the player for all other players
            playerController.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer, spawnPoints.GetItemIndex(mySpawnIndex).transform.eulerAngles.y, spawnPoints.GetItemIndex(mySpawnIndex).transform.eulerAngles.z);
            playerController.DisableMovement();
            StartCoroutine(CountDown());
        }
        LoadingScreen.SetActive(false);
    }

    IEnumerator CountDown()
    {
        countDownText.gameObject.SetActive(true);
        while (waitToStartGameTimer >= 1)
        {
            countDownText.text = waitToStartGameTimer.ToString();
            yield return new WaitForSeconds(1);
            waitToStartGameTimer--;
        }
        countDownText.text = "FIGHT!";
        playerControllerRuntimeSet.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber).EnableMovement();
        yield return new WaitForSeconds(1);
        countDownText.gameObject.SetActive(false);
    }
}