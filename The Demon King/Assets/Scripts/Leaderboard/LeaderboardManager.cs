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
using Cinemachine;


public class LeaderboardManager : MonoBehaviourPun, IOnEventCallback
{
    [SerializeField] private SOMenuData roomData;

    public LeaderboardDataList leaderboardDataList;

    public GameObject LeaderBoardHUD;

    public int PlayerScore;

    [Header("DemonKing Display")]
    [SerializeField] private PlayerLeaderboardPanel DemonKingPanel;

    [Header("Leaderboard Display")]
    public List<PlayerLeaderboardPanel> playerLeaderboardPanel;

    public bool DidAWinOccur = false;

    [Header("Evolution Images To Display")]
    [SerializeField] private Sprite lionImg;
    [SerializeField] private Sprite dragonImg;
    [SerializeField] private Sprite rayImg;

    [Header("Minion Types")]
    [SerializeField] private MinionType redMinion;
    [SerializeField] private MinionType greenMinion;
    [SerializeField] private MinionType blueMinion;


    [SerializeField] private PlayerControllerRuntimeSet players;
    [SerializeField] private SpawnPointRuntimeSet playerCount;



    bool findingPlayers = true;

    int numberOfPlayerToDisplay = 2;

    // private float matchTimer;
    [SerializeField] private float timeToAwardDoubleScore = 300;
    [SerializeField] private TextMeshProUGUI matchTimeText;
    [SerializeField] private GameObject doubleScorePanel;
    [SerializeField] private GameObject doubleScoreWarningFX;

    private bool doubleScoreProced;

    private const byte UpdateLeaderboardEvent = 1;
    private const byte StartMatchTimeEvent = 2;
    private const byte PlayerWonEvent = 3;

    public PlayerControllerRuntimeSet playerControllerRuntimeSet;


    private void Start()
    {
        leaderboardDataList.Data.Clear();

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsSpectator"])
        {
            LeaderBoardHUD.SetActive(false);
            return;
        }

        InputManager.inputActions.DisplayScoreBoard.DisplayScoreBoard.started += DisplayScoreBoard_started;
        InputManager.inputActions.DisplayScoreBoard.DisplayScoreBoard.canceled += DisplayScoreBoard_canceled;

        Hashtable DemonKingScoreHash = new Hashtable();
        DemonKingScoreHash.Add("PlayerScore", PlayerScore);
        PhotonNetwork.LocalPlayer.SetCustomProperties(DemonKingScoreHash);
    }

    private Sprite GetSprite(MinionType minionType)
    {
        if (minionType == redMinion)
            return lionImg;
        else if (minionType == greenMinion)
            return dragonImg;
        else if (minionType == blueMinion)
            return rayImg;
        else
            return null;
    }


    private void Update()
    {
        //NEEDS TO BE CHANGED TO BE A CO IN START SO NOT ALWAYS RUNNING ONCE ITS DONE ITS THING
        if (PhotonNetwork.IsMasterClient)
        {
            if (findingPlayers)
            {
                if (playerCount.Length() == PhotonNetwork.PlayerList.Length)
                {
                    findingPlayers = false;
                    Invoke("InvokeUpdateLeaderboard", 1f);
                }
            }
        }
    }

    void InvokeUpdateLeaderboard()
    {
        RaiseUpdateLeaderboardEvent();
        RaiseStartMatchTimerEvent();
    }


    private void DisplayScoreBoard_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        for (int i = 0; i < leaderboardDataList.Length(); i++)
        {
            playerLeaderboardPanel[i].gameObject.SetActive(true);
        }
    }

    private void DisplayScoreBoard_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        for (int i = numberOfPlayerToDisplay + 1; i <= leaderboardDataList.Length(); i++)
        {
            playerLeaderboardPanel[i].gameObject.SetActive(false);
        }
    }

    public void UpdatePlayerScore(int AmountToIncreaseScoreBy)
    {
        if (!doubleScoreProced)
            PlayerScore += AmountToIncreaseScoreBy;
        else
            PlayerScore += AmountToIncreaseScoreBy * 2;

        Hashtable DemonKingScoreHash = new Hashtable();
        DemonKingScoreHash.Add("PlayerScore", PlayerScore);
        PhotonNetwork.LocalPlayer.SetCustomProperties(DemonKingScoreHash);

        RaiseUpdateLeaderboardEvent();
    }

    public void UpdateLeaderboard()
    {
        //Clear current leaderboard data
        leaderboardDataList.Data.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)player.CustomProperties["IsSpectator"])
                continue;

            LeaderboardData leaderboardData = new LeaderboardData();

            leaderboardData.PlayerNickName = player.NickName;
            //get players score as demon king
            leaderboardData.PlayerScore = (int)player.CustomProperties["PlayerScore"];

            leaderboardData.EvolutionSprite = GetSprite(playerControllerRuntimeSet.GetPlayer(player.ActorNumber).GetComponent<PlayerHealthManager>().MyMinionType);

            leaderboardData.AmITheDemonKing = playerControllerRuntimeSet.GetPlayer(player.ActorNumber).GetComponent<DemonKingEvolution>().AmITheDemonKing;

            leaderboardData.IslocalPlayer = player.IsLocal;

            leaderboardDataList.Data.Add(leaderboardData);
        }

        List<LeaderboardData> sortedPlayerScoreList = leaderboardDataList.Data.OrderByDescending(o => o.PlayerScore).ToList();

        //populate GUI slots with each players name and time as demon king using sorted list
        int i = 0;
        bool wasThereAKing = false;
        foreach (LeaderboardData data in sortedPlayerScoreList)
        {
            if (i <= numberOfPlayerToDisplay)
                playerLeaderboardPanel[i].gameObject.SetActive(true);

            playerLeaderboardPanel[i].PlayerNameText.text = data.PlayerNickName;
            playerLeaderboardPanel[i].DemonKingScoreText.text = data.PlayerScore.ToString();
            playerLeaderboardPanel[i].UpdateSliderValue(data.PlayerScore);
            playerLeaderboardPanel[i].CurrentEvolutionImg.sprite = data.EvolutionSprite;


            if (data.AmITheDemonKing)
            {
                wasThereAKing = true;

                DemonKingPanel.PlayerNameText.gameObject.SetActive(true);
                DemonKingPanel.PlayerNameText.text = data.PlayerNickName;

                DemonKingPanel.DemonKingScoreText.gameObject.SetActive(true);
                DemonKingPanel.DemonKingScoreText.text = data.PlayerScore.ToString();

                DemonKingPanel.FillImg.gameObject.SetActive(true);
                DemonKingPanel.UpdateSliderValue(data.PlayerScore);

                DemonKingPanel.CurrentEvolutionImg.gameObject.SetActive(true);
                DemonKingPanel.CurrentEvolutionImg.sprite = data.EvolutionSprite;

            }
            if (data.PlayerScore >= roomData.PointsToWin)
                DidAWinOccur = true;

            i++;
        }

        if (!wasThereAKing)
        {
            DemonKingPanel.PlayerNameText.gameObject.SetActive(false);
            DemonKingPanel.DemonKingScoreText.gameObject.SetActive(false);
            DemonKingPanel.FillImg.gameObject.SetActive(false);
            DemonKingPanel.CurrentEvolutionImg.gameObject.SetActive(false);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (DidAWinOccur)
            {
                RaisePlayerWonEvent();
            }
        }
    }


    void PlayerWon()
    {
        //Clear current leaderboard data
        leaderboardDataList.Data.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((bool)player.CustomProperties["IsSpectator"])
                continue;

            LeaderboardData leaderboardData = new LeaderboardData();

            leaderboardData.PlayerNickName = player.NickName;
            //get players score as demon king
            leaderboardData.PlayerScore = (int)player.CustomProperties["PlayerScore"];

            leaderboardData.EvolutionSprite = GetSprite(playerControllerRuntimeSet.GetPlayer(player.ActorNumber).GetComponent<PlayerHealthManager>().MyMinionType);

            leaderboardData.AmITheDemonKing = playerControllerRuntimeSet.GetPlayer(player.ActorNumber).GetComponent<DemonKingEvolution>().AmITheDemonKing;

            leaderboardData.IslocalPlayer = player.IsLocal;

            leaderboardData.currentModelName = playerControllerRuntimeSet.GetPlayer(player.ActorNumber).GetComponent<EvolutionManager>().activeEvolution.gameObject.name;

            leaderboardData.playersConsumed = (int)player.CustomProperties["PlayerKills"];

            leaderboardData.MinionsConsumed = (int)player.CustomProperties["MinionKills"];

            leaderboardData.PlayerDeaths = (int)player.CustomProperties["PlayerDeaths"];

            leaderboardDataList.Data.Add(leaderboardData);
        }

        ChangeScene("EndGame");
    }



    public void ChangeScene(string sceneName)
    {
        //Checks if the scene is found within the build settings, otherwise load game as default
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            PhotonNetwork.LoadLevel(sceneName);
        }
        else
        {
            Debug.Log("Scene Not Found in Build Settings");
        }
    }

    public string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time - 60 * minutes;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartMatchTime(double matchTimeStart)
    {
        roomData.GameTimeLimit -= (float)(PhotonNetwork.Time - matchTimeStart);
        StartCoroutine("MatchTimeCountDown");
    }

    IEnumerator MatchTimeCountDown()
    {
        while (roomData.GameTimeLimit >= 0f)
        {
            yield return new WaitForEndOfFrame();
            roomData.GameTimeLimit -= Time.deltaTime;
            matchTimeText.text = FormatTime(roomData.GameTimeLimit);

            if (roomData.GameTimeLimit <= timeToAwardDoubleScore && !doubleScoreProced)
            {
                doubleScoreProced = true;
                doubleScorePanel.SetActive(true);
                doubleScoreWarningFX.SetActive(true);
            }
            if (PhotonNetwork.IsMasterClient)
            {
                if (roomData.GameTimeLimit <= 0)
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
            PlayerWon();
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void OnDestroy()
    {
        InputManager.inputActions.DisplayScoreBoard.DisplayScoreBoard.started -= DisplayScoreBoard_started;
        InputManager.inputActions.DisplayScoreBoard.DisplayScoreBoard.canceled -= DisplayScoreBoard_canceled;
    }
}
