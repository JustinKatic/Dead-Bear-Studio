using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using TMPro;


public class LeaderboardManager : MonoBehaviourPun
{
    PlayerController playerController;

    List<LeaderboardHelper> leaderboardHelper = new List<LeaderboardHelper>();


    private void Start()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponent<PlayerController>();
            playerController.CharacterInputs.DisplayScoreBoard.DisplayScoreBoard.performed += DisplayScoreBoard_performed;
        }
    }

    private void DisplayScoreBoard_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            LeaderboardHelper helper = new LeaderboardHelper();
            helper.PlayerNickName = player.NickName;
            helper.TimeSpentAsDemonKing = (float)player.CustomProperties["TimeAsDemonKing"];
            leaderboardHelper.Add(helper);
        }

        List<LeaderboardHelper> sortedList = leaderboardHelper.OrderByDescending(o => o.TimeSpentAsDemonKing).ToList();

        foreach (var item in sortedList)
        {
            Debug.Log(item.PlayerNickName + "  " + item.TimeSpentAsDemonKing);
        }

        leaderboardHelper.Clear();
    }
}
