using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndGameLeaderboardManager : MonoBehaviourPun
{
    public LeaderboardDataList leaderboardDataList;
    [Header("Leaderboard Display")]

    public List<EndGameLeaderboardPanel> playerEndGameLeaderboardPanel = new List<EndGameLeaderboardPanel>();

    public GameObject EndgameLeaderboardBackground;

    [SerializeField] private List<GameObject> playerModels;

    [SerializeField] private GameObject spawnPos1;
    [SerializeField] private GameObject spawnPos2;
    [SerializeField] private GameObject spawnPos3;

    private void Start()
    {
        DisplayEndGameBoard();
    }

    void DisplayEndGameBoard()
    {
        List<LeaderboardData> sortedPlayerScoreList = leaderboardDataList.Data.OrderByDescending(o => o.PlayerScore).ToList();
        List<LeaderboardData> sortedPlayerConsumesList = leaderboardDataList.Data.OrderByDescending(o => o.playersConsumed).ToList();
        List<LeaderboardData> sortedMinionConsumesList = leaderboardDataList.Data.OrderByDescending(o => o.MinionsConsumed).ToList();
        List<LeaderboardData> sortedDeathsList = leaderboardDataList.Data.OrderByDescending(o => o.PlayerDeaths).ToList();


        int i = 0;
        foreach (LeaderboardData data in sortedPlayerScoreList)
        {
            playerEndGameLeaderboardPanel[i].DemonKingScoreText.text = data.PlayerScore.ToString();
            playerEndGameLeaderboardPanel[i].PlayerNameText.text = data.PlayerNickName;
            playerEndGameLeaderboardPanel[i].PlayerDeathsText.text = data.PlayerDeaths.ToString();
            playerEndGameLeaderboardPanel[i].PlayersConsumedText.text = data.playersConsumed.ToString();
            playerEndGameLeaderboardPanel[i].MinionsConsumedText.text = data.MinionsConsumed.ToString();

            if (leaderboardDataList.Data[i].PlayerScore == leaderboardDataList.Data[0].PlayerScore)
                playerEndGameLeaderboardPanel[i].HighestScoreImg.SetActive(true);

            if (leaderboardDataList.Data[i].playersConsumed == sortedPlayerConsumesList[0].playersConsumed)
                playerEndGameLeaderboardPanel[i].HighestPlayerConsumesImg.SetActive(true);

            if (leaderboardDataList.Data[i].MinionsConsumed == sortedMinionConsumesList[0].MinionsConsumed)
                playerEndGameLeaderboardPanel[i].HighestMinionConsumesImg.SetActive(true);

            if (leaderboardDataList.Data[i].PlayerDeaths == sortedDeathsList[0].PlayerDeaths)
                playerEndGameLeaderboardPanel[i].HighestDeathsImg.SetActive(true);

            playerEndGameLeaderboardPanel[i].gameObject.SetActive(true);

            if (i == 0)
            {
                Instantiate(GetPlayerModel(data.currentModelTag), spawnPos1.transform.position, spawnPos1.transform.rotation);
            }
            else if (i == 1)
            {
                Instantiate(GetPlayerModel(data.currentModelTag), spawnPos2.transform.position, spawnPos2.transform.rotation);
            }
            else if (i == 2)
            {
                Instantiate(GetPlayerModel(data.currentModelTag), spawnPos2.transform.position, spawnPos2.transform.rotation);
            }

            i++;
        }
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
}
