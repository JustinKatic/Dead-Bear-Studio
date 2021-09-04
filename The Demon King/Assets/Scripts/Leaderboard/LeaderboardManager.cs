using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;

struct LeaderBoardList
{
    public string PlayerNickName;
    public int DemonKingScore;
    public Image EvolutionImg;
    public bool AmITheDemonKing;
}


public class LeaderboardManager : MonoBehaviourPun
{
    PlayerController playerController;
    public GameObject LeaderBoardHUD;

    public int DemonKingScore;
    public IntSO DemonKingScoreRequiredToWin;

    [Header("DemonKing Display")]
    [SerializeField] private PlayerLeaderboardPanel DemonKingPanel;

    [Header("Leaderboard Display")]
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

    [HideInInspector] public bool leaderboardEnabed = false;

    private GameObject[] players;
    bool findingPlayers = true;

    int numberOfPlayerToDisplay = 2;





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

            Hashtable DemonKingScoreHash = new Hashtable();
            DemonKingScoreHash.Add("DemonKingScore", DemonKingScore);
            PhotonNetwork.LocalPlayer.SetCustomProperties(DemonKingScoreHash);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (findingPlayers)
            {
                players = GameObject.FindGameObjectsWithTag("PlayerParent");
                if (players.Length == PhotonNetwork.PlayerList.Length)
                {
                    findingPlayers = false;
                    Invoke("UpdateLeaderboard", 1);
                }
            }
        }
    }

    private void DisplayScoreBoard_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Count(); i++)
        {
            playerLeaderboardSlot[i].gameObject.SetActive(true);
        }
    }

    private void DisplayScoreBoard_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        for (int i = numberOfPlayerToDisplay + 1; i <= PhotonNetwork.PlayerList.Count(); i++)
        {
            playerLeaderboardSlot[i].gameObject.SetActive(false);
        }
    }

    public void UpdateDemonKingScore(int AmountToIncreaseScoreBy)
    {
        DemonKingScore += AmountToIncreaseScoreBy;
        Hashtable DemonKingScoreHash = new Hashtable();
        DemonKingScoreHash.Add("DemonKingScore", DemonKingScore);
        PhotonNetwork.LocalPlayer.SetCustomProperties(DemonKingScoreHash);

        foreach (var player in players)
        {
            if (player != null)
                UpdateLeadboardNetworked(player);
        }
    }


    public void UpdateLeadboardNetworked(GameObject player)
    {
        player.GetPhotonView().RPC("UpdateLeadboardNetworked_RPC", RpcTarget.All);
    }

    [PunRPC]
    void UpdateLeadboardNetworked_RPC()
    {
        if (photonView.IsMine)
            Invoke("UpdateLeaderboardInvoke", .5f);
    }

    void UpdateLeaderboardInvoke()
    {
        UpdateLeaderboard();
    }


    public void UpdateLeaderboard()
    {
        //Clear current leaderboard data
        leaderBoardList.Clear();

        Debug.Log("Updating leaderboard");

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            LeaderBoardList dataToEnterIntoLeaderboardList = new LeaderBoardList();
            //get players name
            dataToEnterIntoLeaderboardList.PlayerNickName = player.NickName;
            //get players time as demon king
            dataToEnterIntoLeaderboardList.DemonKingScore = (int)player.CustomProperties["DemonKingScore"];

            dataToEnterIntoLeaderboardList.EvolutionImg = GetImage(GameManager.instance.GetPlayer(player.ActorNumber).GetComponent<PlayerHealthManager>().MyMinionType);

            dataToEnterIntoLeaderboardList.AmITheDemonKing = GameManager.instance.GetPlayer(player.ActorNumber).GetComponent<DemonKingEvolution>().AmITheDemonKing;
            //Add info into leaderboard list
            leaderBoardList.Add(dataToEnterIntoLeaderboardList);
        }

        //Sort leader board list by time spent as demon king and store in sorted leader board list
        List<LeaderBoardList> sortedLeaderboardList = leaderBoardList.OrderByDescending(o => o.DemonKingScore).ToList();

        //populate GUI slots with each players name and time as demon king using sorted list
        int i = 0;
        bool wasThereAKing = false;
        foreach (LeaderBoardList player in sortedLeaderboardList)
        {
            if (i <= numberOfPlayerToDisplay)
                playerLeaderboardSlot[i].gameObject.SetActive(true);

            playerLeaderboardSlot[i].PlayerNameText.text = player.PlayerNickName;
            playerLeaderboardSlot[i].DemonKingScoreText.text = player.DemonKingScore.ToString();
            playerLeaderboardSlot[i].UpdateSliderValue(player.DemonKingScore);
            playerLeaderboardSlot[i].CurrentEvolutionImg.sprite = player.EvolutionImg.sprite;

            if (player.AmITheDemonKing)
            {
                wasThereAKing = true;
                DemonKingPanel.gameObject.SetActive(true);
                DemonKingPanel.PlayerNameText.text = player.PlayerNickName;
                DemonKingPanel.DemonKingScoreText.text = player.DemonKingScore.ToString();
                DemonKingPanel.UpdateSliderValue(player.DemonKingScore);
                DemonKingPanel.CurrentEvolutionImg.sprite = player.EvolutionImg.sprite;
                if (player.DemonKingScore >= DemonKingScoreRequiredToWin.value)
                    DidAWinOccur = true;
            }
            i++;
        }

        if (!wasThereAKing)
            DemonKingPanel.gameObject.SetActive(false);

        if (DidAWinOccur)
        {
            foreach (var player in players)
            {
                if (player != null)
                    ReturnToLobby();
            }
        }

    }

    void ReturnToLobby()
    {
        photonView.RPC("ReturnToLobby_RPC", RpcTarget.All);
    }

    [PunRPC]
    void ReturnToLobby_RPC()
    {
        if (photonView.IsMine)
            StartCoroutine(ReturnToLobbyCo());
    }

    IEnumerator ReturnToLobbyCo()
    {
        yield return new WaitForSeconds(4f);

        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;
        {
            PhotonNetwork.LoadLevel("Menu");
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
