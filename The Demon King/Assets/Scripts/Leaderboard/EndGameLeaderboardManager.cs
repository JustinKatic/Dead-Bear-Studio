using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameLeaderboardManager : MonoBehaviourPun
{
    public SOMenuData roomdata;

    public float TimeBetweenEachAnim = 0.5f;

    public LeaderboardDataList leaderboardDataList;
    [Header("Leaderboard Display")]

    public List<EndGameLeaderboardPanel> playerEndGameLeaderboardPanel = new List<EndGameLeaderboardPanel>();

    public GameObject EndgameLeaderboardBackground;

    [SerializeField] private List<GameObject> playerModels;

    [SerializeField] private GameObject[] spawnPositions;

    [SerializeField] private TextMeshProUGUI playerNameDisplayPrefab;

    [FMODUnity.EventRef]
    public string ChainSound;

    private FMOD.Studio.EventInstance chainSoundInstance;

    [SerializeField] private Image fadeInImg;
    private float fadeInLerpTime = 2;




    private void Start()
    {
        StartCoroutine(DisplayEndGameBoard());
        StartCoroutine(ReturnToLobby());
        StartCoroutine(LerpFadeoutScreenImg());
    }

    IEnumerator LerpFadeoutScreenImg()
    {
        fadeInImg.gameObject.SetActive(true);
        float lerpTime = 0;

        while (lerpTime < fadeInLerpTime)
        {
            float valToBeLerped = Mathf.Lerp(0, 1, (lerpTime / fadeInLerpTime));
            lerpTime += Time.deltaTime;
            fadeInImg.material.SetFloat("_EffectTime", valToBeLerped);
            yield return null;
        }
        fadeInImg.material.SetFloat("_EffectTime", 1);
    }


    IEnumerator DisplayEndGameBoard()
    {
        chainSoundInstance = FMODUnity.RuntimeManager.CreateInstance(ChainSound);

        List<LeaderboardData> sortedPlayerScoreList = leaderboardDataList.Data.OrderByDescending(o => o.PlayerScore).ToList();
        List<LeaderboardData> sortedPlayerConsumesList = leaderboardDataList.Data.OrderByDescending(o => o.playersConsumed).ToList();
        List<LeaderboardData> sortedMinionConsumesList = leaderboardDataList.Data.OrderByDescending(o => o.MinionsConsumed).ToList();
        List<LeaderboardData> sortedDeathsList = leaderboardDataList.Data.OrderByDescending(o => o.PlayerDeaths).ToList();

        playerNameDisplayPrefab.text = sortedPlayerScoreList[0].PlayerNickName;

        int i = 0;

        foreach (LeaderboardData data in sortedPlayerScoreList)
        {
            GameObject model = Instantiate(GetPlayerModel(data.currentModelName + "EndGame"), spawnPositions[i].transform.position, spawnPositions[i].transform.rotation);
            model.GetComponentInChildren<TextMeshProUGUI>().text = data.PlayerNickName;
            if (i >= 3)
                model.GetComponent<Animator>().SetBool("4thPlaceAnim", true);

            i++;
        }

        yield return new WaitForSeconds(fadeInLerpTime);

        chainSoundInstance.start();

        i = 0;
        foreach (LeaderboardData data in sortedPlayerScoreList)
        {
            playerEndGameLeaderboardPanel[i].DemonKingScoreText.text = data.PlayerScore.ToString();
            playerEndGameLeaderboardPanel[i].PlayerNameText.text = data.PlayerNickName;
            playerEndGameLeaderboardPanel[i].PlayerDeathsText.text = data.PlayerDeaths.ToString();
            playerEndGameLeaderboardPanel[i].PlayersConsumedText.text = data.playersConsumed.ToString();
            playerEndGameLeaderboardPanel[i].MinionsConsumedText.text = data.MinionsConsumed.ToString();

            if (sortedPlayerScoreList[i].PlayerScore == sortedPlayerScoreList[0].PlayerScore)
                playerEndGameLeaderboardPanel[i].HighestScoreImg.SetActive(true);

            if (sortedPlayerScoreList[i].playersConsumed == sortedPlayerConsumesList[0].playersConsumed)
                playerEndGameLeaderboardPanel[i].HighestPlayerConsumesImg.SetActive(true);

            if (sortedPlayerScoreList[i].MinionsConsumed == sortedMinionConsumesList[0].MinionsConsumed)
                playerEndGameLeaderboardPanel[i].HighestMinionConsumesImg.SetActive(true);

            if (sortedPlayerScoreList[i].PlayerDeaths == sortedDeathsList[0].PlayerDeaths)
                playerEndGameLeaderboardPanel[i].HighestDeathsImg.SetActive(true);

            playerEndGameLeaderboardPanel[i].gameObject.SetActive(true);

            i++;
            yield return new WaitForSeconds(TimeBetweenEachAnim);
        }
        chainSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    GameObject GetPlayerModel(string modelName)
    {
        for (int i = 0; i < playerModels.Count; i++)
        {
            if (playerModels[i].name == modelName)
                return playerModels[i];
        }
        return null;
    }

    IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(20f);
        if (PhotonNetwork.IsMasterClient)
            ChangeScene("Menu");
    }

    public void ChangeScene(string sceneName)
    {
        photonView.RPC("ChangeScene_RPC", RpcTarget.All, sceneName);
    }

    [PunRPC]
    public void ChangeScene_RPC(string sceneName)
    {
        //Checks if the scene is found within the build settings, otherwise load game as default
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            if (roomdata.InTutorial)
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(sceneName);
            }
            else
                PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {
            Debug.Log("Scene Not Found in Build Settings");
        }
    }
}
