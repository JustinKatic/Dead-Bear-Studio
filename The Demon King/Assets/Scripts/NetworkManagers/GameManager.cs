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

    public GameObject[] spawnPoints;
    public int currentSpawnIndex = 0;

    private int playersInGame;
    private int startGameTimer = 5;

    private int mySpawnIndex;
    public GameObject LoadingScreen;
    public Slider loadingBar;
    public PlayerControllerRuntimeSet playerControllerRuntimeSet;
    public Sprite[] startGameTimerPrefabs;
    public Image startGameTimerImg;
    [SerializeField] private Image loadingScreenMat;


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
        StartCoroutine(LerpLoadingScreenImg());
    }


    IEnumerator LerpLoadingScreenImg()
    {
        loadingBar.gameObject.SetActive(false);

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsSpectator"] == true)
        {
            //LOAD SPECTAOR PREFAB
            GameObject spectatorPrefab = PhotonNetwork.Instantiate(spectatorPrefabLocation, spawnPoints[mySpawnIndex].transform.position, spawnPoints[mySpawnIndex].transform.rotation);
            SpectatorCamera spectatorCamera = spectatorPrefab.GetComponent<SpectatorCamera>();

            spectatorCamera.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
        }
        else
        {
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[mySpawnIndex].transform.position, spawnPoints[mySpawnIndex].transform.rotation);

            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            // initialize the player for all other players
            playerController.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer, spawnPoints[mySpawnIndex].transform.eulerAngles.y, spawnPoints[mySpawnIndex].transform.eulerAngles.z);
            playerController.DisableMovement();
        }

        float lerpTime = 0;


        while (lerpTime < 2)
        {
            float valToBeLerped = Mathf.Lerp(0, 1, (lerpTime / 2));
            lerpTime += Time.deltaTime;
            loadingScreenMat.material.SetFloat("_EffectTime", valToBeLerped);
            yield return null;
        }
        loadingScreenMat.material.SetFloat("_EffectTime", 1);

        LoadingScreen.SetActive(false);

        if (SceneManager.GetActiveScene().name != "Tutorial" && ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsSpectator"] == false))
            StartCoroutine(CountDown());
    }


    IEnumerator CountDown()
    {
        startGameTimerImg.gameObject.SetActive(true);
        while (startGameTimer >= 1)
        {
            startGameTimerImg.sprite = startGameTimerPrefabs[startGameTimer - 1];
            yield return new WaitForSeconds(1);
            startGameTimer--;
        }
        playerControllerRuntimeSet.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber).EnableMovement();
        startGameTimerImg.gameObject.SetActive(false);
    }
}