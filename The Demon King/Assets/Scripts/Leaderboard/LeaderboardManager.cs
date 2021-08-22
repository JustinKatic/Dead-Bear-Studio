using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;


struct LeaderBoardList
{
    public string PlayerNickName;
    public float TimeSpentAsDemonKing;
    public Image EvolutionImg;
}


public class LeaderboardManager : MonoBehaviourPun
{
    PlayerController playerController;
    public GameObject LeaderBoardHUD;

    List<LeaderBoardList> leaderBoardList = new List<LeaderBoardList>();
    public List<PlayerLeaderboardPanel> playerLeaderboardSlot = new List<PlayerLeaderboardPanel>();

    public bool DidAWinOccur = false;

    [Header("Evolution Images To Display")]
    [SerializeField] private Image red;
    [SerializeField] private Image green;
    [SerializeField] private Image blue;

    [Header("Minion Types")]
    [SerializeField] private MinionType redMinion;
    [SerializeField] private MinionType greenMinion;
    [SerializeField] private MinionType blueMinion;

    bool leaderboardEnabed = false;


    private Image GetImage(MinionType minionType)
    {
        if (minionType == redMinion)
            return red;
        else if (minionType == greenMinion)
            return green;
        else if (minionType == blueMinion)
            return blue;
        else
            return null;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            playerController = GetComponentInParent<PlayerController>();
            playerController.CharacterInputs.DisplayScoreBoard.DisplayScoreBoard.started += DisplayScoreBoard_started;
            playerController.CharacterInputs.DisplayScoreBoard.DisplayScoreBoard.canceled += DisplayScoreBoard_canceled;
        }
    }

    private void DisplayScoreBoard_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        leaderboardEnabed = true;
        DisplayLeaderboard();
    }

    private void DisplayScoreBoard_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!DidAWinOccur)
        {
            leaderboardEnabed = false;
            LeaderBoardHUD.SetActive(false);
        }
    }



    public void DisplayLeaderboard()
    {
        if (leaderboardEnabed == false)
            return;

        //Clear current leaderboard data
        leaderBoardList.Clear();

        LeaderBoardHUD.SetActive(true);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            LeaderBoardList dataToEnterIntoLeaderboardList = new LeaderBoardList();
            //get players name
            dataToEnterIntoLeaderboardList.PlayerNickName = player.NickName;
            //get players time as demon king
            dataToEnterIntoLeaderboardList.TimeSpentAsDemonKing = (float)player.CustomProperties["TimeAsDemonKing"];

            dataToEnterIntoLeaderboardList.EvolutionImg = GetImage(GameManager.instance.GetPlayer(player.ActorNumber).GetComponent<PlayerHealthManager>().MyMinionType);
            //Add info into leaderboard list
            leaderBoardList.Add(dataToEnterIntoLeaderboardList);
        }

        //Sort leader board list by time spent as demon king and store in sorted leader board list
        List<LeaderBoardList> sortedLeaderboardList = leaderBoardList.OrderByDescending(o => o.TimeSpentAsDemonKing).ToList();

        //populate GUI slots with each players name and time as demon king using sorted list
        int i = 0;
        foreach (LeaderBoardList player in sortedLeaderboardList)
        {
            playerLeaderboardSlot[i].gameObject.SetActive(true);
            playerLeaderboardSlot[i].PlayerName.text = player.PlayerNickName;
            playerLeaderboardSlot[i].TimeSpentAsDemonKing.text = Mathf.Round(player.TimeSpentAsDemonKing).ToString();
            playerLeaderboardSlot[i].UpdateSliderValue((int)Mathf.Round(player.TimeSpentAsDemonKing));
            playerLeaderboardSlot[i].CurrentEvolutionImg.sprite = player.EvolutionImg.sprite;
            i++;
        }

        Invoke("DisplayLeaderboard", 1);

        //Display the leaderboard

        if (DidAWinOccur)
        {
            StartCoroutine(ReturnToLobby());
        }
    }

    IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(4f);

        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;
        {
            Destroy(NetworkManager.instance.gameObject);
            PhotonNetwork.LoadLevel("Menu");
        }
    }
}
