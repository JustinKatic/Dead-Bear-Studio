using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


struct EndGameLeaderBoardList
{
    public string PlayerNickName;
    public int DemonKingScore;
    public int playersConsumed;
    public int MinionsConsumed;
    public int PlayerDeaths;
    public bool IslocalPlayer;
}

public class EndGameLeaderboardManager : MonoBehaviourPun
{
    [Header("Leaderboard Display")]
    List<EndGameLeaderBoardList> endGameLeaderBoardList = new List<EndGameLeaderBoardList>();
    public List<EndGameLeaderboardPanel> playerEndGameLeaderboardPanel = new List<EndGameLeaderboardPanel>();

    public GameObject EndgameLeaderboardBackground;


    public void DisplayEndgameLeaderboard(float matchTime, float TimeAsSlime, float TimeAsLion, float TimeAsLionKing, float TimeAsRay, float TimeAsRayKing, float TimeAsDragon, float TimeAsDragonKing, int slimeDmg, int LionDmg, int rayDmg, int dragonDmg)
    {
        EndgameLeaderboardBackground.SetActive(true);

        int i = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsSpectator"])
                continue;

            playerEndGameLeaderboardPanel[i].gameObject.SetActive(true);
            i++;
            EndGameLeaderBoardList dataToEnterIntoEndGameLeaderboardList = new EndGameLeaderBoardList();
            //get players name
            dataToEnterIntoEndGameLeaderboardList.PlayerNickName = player.NickName;
            //get players time as demon king
            dataToEnterIntoEndGameLeaderboardList.DemonKingScore = (int)player.CustomProperties["DemonKingScore"];
            dataToEnterIntoEndGameLeaderboardList.PlayerDeaths = (int)player.CustomProperties["PlayerDeaths"];
            dataToEnterIntoEndGameLeaderboardList.playersConsumed = (int)player.CustomProperties["PlayerKills"];
            dataToEnterIntoEndGameLeaderboardList.MinionsConsumed = (int)player.CustomProperties["MinionKills"];
            dataToEnterIntoEndGameLeaderboardList.IslocalPlayer = player.IsLocal;

            //Add info into leaderboard list
            endGameLeaderBoardList.Add(dataToEnterIntoEndGameLeaderboardList);
        }

        List<EndGameLeaderBoardList> sortedScoreList = endGameLeaderBoardList.OrderByDescending(o => o.DemonKingScore).ToList();
        List<EndGameLeaderBoardList> sortedPlayerConsumesList = endGameLeaderBoardList.OrderByDescending(o => o.playersConsumed).ToList();
        List<EndGameLeaderBoardList> sortedMinionConsumesList = endGameLeaderBoardList.OrderByDescending(o => o.MinionsConsumed).ToList();
        List<EndGameLeaderBoardList> sortedDeathsList = endGameLeaderBoardList.OrderByDescending(o => o.PlayerDeaths).ToList();


        int p = 0;
        foreach (EndGameLeaderBoardList player in sortedScoreList)
        {
            playerEndGameLeaderboardPanel[p].DemonKingScoreText.text = player.DemonKingScore.ToString();
            playerEndGameLeaderboardPanel[p].PlayerNameText.text = player.PlayerNickName;
            playerEndGameLeaderboardPanel[p].PlayerDeathsText.text = player.PlayerDeaths.ToString();
            playerEndGameLeaderboardPanel[p].PlayersConsumedText.text = player.playersConsumed.ToString();
            playerEndGameLeaderboardPanel[p].MinionsConsumedText.text = player.MinionsConsumed.ToString();

            if (sortedScoreList[p].DemonKingScore == sortedScoreList[0].DemonKingScore)
                playerEndGameLeaderboardPanel[p].HighestScoreImg.SetActive(true);

            if (sortedScoreList[p].playersConsumed == sortedPlayerConsumesList[0].playersConsumed)
                playerEndGameLeaderboardPanel[p].HighestPlayerConsumesImg.SetActive(true);

            if (sortedScoreList[p].MinionsConsumed == sortedMinionConsumesList[0].MinionsConsumed)
                playerEndGameLeaderboardPanel[p].HighestMinionConsumesImg.SetActive(true);

            if (sortedScoreList[p].PlayerDeaths == sortedDeathsList[0].PlayerDeaths)
                playerEndGameLeaderboardPanel[p].HighestDeathsImg.SetActive(true);

            if (player.IslocalPlayer)
            {
                DemonKingInGameAnalytics.instance.DemonKingScore = player.DemonKingScore;
                DemonKingInGameAnalytics.instance.PlayerDeaths = player.PlayerDeaths;
                DemonKingInGameAnalytics.instance.PlayerConsumed = player.playersConsumed;
                DemonKingInGameAnalytics.instance.MinionsConsumed = player.MinionsConsumed;
                DemonKingInGameAnalytics.instance.MatchDuration = matchTime;

                DemonKingInGameAnalytics.instance.TimeSpentAsSlime = TimeAsSlime;

                DemonKingInGameAnalytics.instance.TimeSpentAsLion = TimeAsLion;
                DemonKingInGameAnalytics.instance.TimeSpentAsLionKing = TimeAsLionKing;

                DemonKingInGameAnalytics.instance.TimeSpentAsRay = TimeAsRay;
                DemonKingInGameAnalytics.instance.TimeSpentAsRayKing = TimeAsRayKing;

                DemonKingInGameAnalytics.instance.TimeSpentAsDragon = TimeAsDragon;
                DemonKingInGameAnalytics.instance.TimeSpentAsDragonKing = TimeAsDragonKing;

                DemonKingInGameAnalytics.instance.SlimeDamageOutput = slimeDmg;
                DemonKingInGameAnalytics.instance.LionDamageOutput = LionDmg;
                DemonKingInGameAnalytics.instance.RayDamageOutput = rayDmg;
                DemonKingInGameAnalytics.instance.DragonDamageOutput = dragonDmg;
            }

            p++;
        }
    }
}
