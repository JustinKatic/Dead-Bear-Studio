using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using TMPro;


struct LeaderboardContainerInfo
{
    public string PlayerNickName;
    public float TimeSpentAsDemonKing;
}


public class LeaderboardManager : MonoBehaviourPun
{
    PlayerController playerController;
    public GameObject LeaderBoardHUD;

    List<LeaderboardContainerInfo> leaderboardPlayerContainerInfo = new List<LeaderboardContainerInfo>();
    public List<PlayerLeaderboardPanel> playerLeaderboardSlot = new List<PlayerLeaderboardPanel>();




    private void Start()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponentInParent<PlayerController>();
            playerController.CharacterInputs.DisplayScoreBoard.DisplayScoreBoard.started += DisplayScoreBoard_started;
            playerController.CharacterInputs.DisplayScoreBoard.DisplayScoreBoard.canceled += DisplayScoreBoard_canceled;
        }
    }

    private void DisplayScoreBoard_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        LeaderBoardHUD.SetActive(false);
        leaderboardPlayerContainerInfo.Clear();
    }

    private void DisplayScoreBoard_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            LeaderboardContainerInfo leaderboardContainerInfo = new LeaderboardContainerInfo();
            leaderboardContainerInfo.PlayerNickName = player.NickName;
            leaderboardContainerInfo.TimeSpentAsDemonKing = (float)player.CustomProperties["TimeAsDemonKing"];
            leaderboardPlayerContainerInfo.Add(leaderboardContainerInfo);
        }

        List<LeaderboardContainerInfo> playerSortedList = leaderboardPlayerContainerInfo.OrderByDescending(o => o.TimeSpentAsDemonKing).ToList();

        int i = 0;
        foreach (LeaderboardContainerInfo player in playerSortedList)
        {
            playerLeaderboardSlot[i].PlayerName.text = player.PlayerNickName;
            playerLeaderboardSlot[i].TimeSpentAsDemonKing.text = Mathf.Round(player.TimeSpentAsDemonKing).ToString();
            i++;
        }

        LeaderBoardHUD.SetActive(true);

    }
}
