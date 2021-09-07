using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using ExitGames.Client.Photon;

struct LeaderBoardList
{
    public string PlayerNickName;
    public int DemonKingScore;
    public Image EvolutionImg;
    public bool AmITheDemonKing;
}


public class LeaderboardManager : MonoBehaviourPun, IOnEventCallback
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

    [SerializeField] private float matchTime = 900;
    [SerializeField] private float timeToAwardDoubleScore = 300;
    [SerializeField] private TextMeshProUGUI matchTimeText;
    [SerializeField] private GameObject doubleScorePanel;
    [SerializeField] private GameObject winPanel;
    private bool doubleScoreProced;

    private const byte UpdateLeaderboardEvent = 1;
    private const byte StartMatchTimeEvent = 2;
    private const byte PlayerWonEvent = 3;




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
        else
            Destroy(this);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (findingPlayers)
            {
                players = GameObject.FindGameObjectsWithTag("PlayerParent");
                if (players.Length == PhotonNetwork.PlayerList.Length)
                {
                    findingPlayers = false;
                    RaiseUpdateLeaderboardEvent();
                    RaiseStartMatchTimerEvent();
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
        if (!doubleScoreProced)
            DemonKingScore += AmountToIncreaseScoreBy;
        else
            DemonKingScore += AmountToIncreaseScoreBy * 2;

        Hashtable DemonKingScoreHash = new Hashtable();
        DemonKingScoreHash.Add("DemonKingScore", DemonKingScore);
        PhotonNetwork.LocalPlayer.SetCustomProperties(DemonKingScoreHash);

        RaiseUpdateLeaderboardEvent();
    }


    public void UpdateLeaderboard()
    {
        //Clear current leaderboard data
        leaderBoardList.Clear();

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

        if (PhotonNetwork.IsMasterClient)
        {
            if (DidAWinOccur)
            {
                RaisePlayerWonEvent();
            }
        }
    }


    IEnumerator ReturnToLobbyCo()
    {
        winPanel.SetActive(true);
        yield return new WaitForSeconds(4f);

        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;

        PhotonNetwork.LoadLevel("Menu");
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartMatchTime(double matchTimeStart)
    {
        matchTime -= (float)(PhotonNetwork.Time - matchTimeStart);
        StartCoroutine("MatchTimeCountDown");
    }

    IEnumerator MatchTimeCountDown()
    {
        while (matchTime >= 0f)
        {
            yield return new WaitForEndOfFrame();
            matchTime -= Time.deltaTime;
            matchTimeText.text = FormatTime(matchTime);

            if (matchTime <= timeToAwardDoubleScore && !doubleScoreProced)
            {
                doubleScoreProced = true;
                doubleScorePanel.SetActive(true);
            }
            if (PhotonNetwork.IsMasterClient)
            {
                if (matchTime <= 0)
                    RaisePlayerWonEvent();
            }
        }
    }



    public void RaisePlayerWonEvent()
    {
        RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(PlayerWonEvent, null, raiseEventOption, sendOptions);
    }

    public void RaiseStartMatchTimerEvent()
    {
        object[] data = new object[] { PhotonNetwork.Time };
        RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(StartMatchTimeEvent, data, raiseEventOption, sendOptions);
    }

    public void RaiseUpdateLeaderboardEvent()
    {
        RaiseEventOptions raiseEventOption = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(UpdateLeaderboardEvent, null, raiseEventOption, sendOptions);
    }


    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == UpdateLeaderboardEvent)
            Invoke("UpdateLeaderboard", .5f);

        else if (photonEvent.Code == StartMatchTimeEvent)
        {
            object[] data = (object[])photonEvent.CustomData;
            StartMatchTime((double)data[0]);
        }
        else if (photonEvent.Code == PlayerWonEvent)
            StartCoroutine(ReturnToLobbyCo());
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
