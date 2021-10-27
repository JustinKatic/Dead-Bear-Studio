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

    void DisplayEndGameBoard()
    {
        List<LeaderboardData> sortedPlayerConsumesList = leaderboardDataList.Data.OrderByDescending(o => o.playersConsumed).ToList();
        List<LeaderboardData> sortedMinionConsumesList = leaderboardDataList.Data.OrderByDescending(o => o.MinionsConsumed).ToList();
        List<LeaderboardData> sortedDeathsList = leaderboardDataList.Data.OrderByDescending(o => o.PlayerDeaths).ToList();


        int i = 0;
        foreach (LeaderboardData data in leaderboardDataList.Data)
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

            i++;
        }
    }
}
