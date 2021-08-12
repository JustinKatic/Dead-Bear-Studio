using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;


public class LeaderboardManager : MonoBehaviourPun
{
    PlayerController playerController;


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
            Debug.Log(player.NickName);
            Debug.Log(player.CustomProperties["TimeAsDemonKing"].ToString());
        }
    }
}
