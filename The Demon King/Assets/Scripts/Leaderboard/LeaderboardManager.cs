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

struct LeaderBoardList
{
    public string PlayerNickName;
    public int DemonKingScore;
    public Image EvolutionImg;
    public bool AmITheDemonKing;
    public bool IslocalPlayer;
}


public class LeaderboardManager : MonoBehaviourPun, IOnEventCallback
{
    [SerializeField] private SOMenuData roomData;

    PlayerController playerController;
    EvolutionManager evolutionManager;
    public GameObject LeaderBoardHUD;

    public int DemonKingScore;
    private int DemonKingScoreRequiredToWin;

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


    [SerializeField] private GameObjectRuntimeSet players;
    bool findingPlayers = true;

    int numberOfPlayerToDisplay = 2;

    private float matchTime;
    [SerializeField] private float timeToAwardDoubleScore = 300;
    [SerializeField] private TextMeshProUGUI matchTimeText;
    [SerializeField] private GameObject doubleScorePanel;




    private CinemachineVirtualCamera podiumVCam;

    private Transform podium1;
    private Transform podium2;
    private Transform podium3;

    [SerializeField] GameObject playerHUD;
    [SerializeField] GameObject playerNameOverhead;
    [SerializeField] GameObject demonKingVFX;
    [SerializeField] GameObject overheadNameTag;

    private PhotonTransformViewClassic transformViewClassic;


    private bool doubleScoreProced;

    private int playerPositionOnScoreboard;

    private const byte UpdateLeaderboardEvent = 1;
    private const byte StartMatchTimeEvent = 2;
    private const byte PlayerWonEvent = 3;


    private void Start()
    {
        if (photonView.IsMine)
        {
            matchTime = roomData.GameTimeLimit;
            DemonKingScoreRequiredToWin = roomData.PointsToWin;

            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["IsSpectator"])
                return;

            playerController = GetComponentInParent<PlayerController>();
            evolutionManager = GetComponentInParent<EvolutionManager>();
            transformViewClassic = GetComponent<PhotonTransformViewClassic>();
            playerController.CharacterInputs.DisplayScoreBoard.DisplayScoreBoard.started += DisplayScoreBoard_started;
            playerController.CharacterInputs.DisplayScoreBoard.DisplayScoreBoard.canceled += DisplayScoreBoard_canceled;

            podium1 = GameObject.Find("Podium1").transform;
            podium2 = GameObject.Find("Podium2").transform;
            podium3 = GameObject.Find("Podium3").transform;

            podiumVCam = GameObject.Find("PodiumVCam").GetComponent<CinemachineVirtualCamera>();


            Hashtable DemonKingScoreHash = new Hashtable();
            DemonKingScoreHash.Add("DemonKingScore", DemonKingScore);
            PhotonNetwork.LocalPlayer.SetCustomProperties(DemonKingScoreHash);
        }
        else
            Destroy(this);
    }

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


    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (findingPlayers)
            {
                if (players.Length() == PhotonNetwork.PlayerList.Length)
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
        for (int i = 0; i < leaderBoardList.Count; i++)
        {
            playerLeaderboardSlot[i].gameObject.SetActive(true);
        }
    }

    private void DisplayScoreBoard_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        for (int i = numberOfPlayerToDisplay + 1; i <= leaderBoardList.Count; i++)
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
            if ((bool)player.CustomProperties["IsSpectator"])
                continue;

            LeaderBoardList dataToEnterIntoLeaderboardList = new LeaderBoardList();
            //get players name
            dataToEnterIntoLeaderboardList.PlayerNickName = player.NickName;
            //get players time as demon king
            dataToEnterIntoLeaderboardList.DemonKingScore = (int)player.CustomProperties["DemonKingScore"];

            dataToEnterIntoLeaderboardList.EvolutionImg = GetImage(GameManager.instance.GetPlayer((int)player.CustomProperties["PlayerId"]).GetComponent<PlayerHealthManager>().MyMinionType);

            dataToEnterIntoLeaderboardList.AmITheDemonKing = GameManager.instance.GetPlayer((int)player.CustomProperties["PlayerId"]).GetComponent<DemonKingEvolution>().AmITheDemonKing;

            dataToEnterIntoLeaderboardList.IslocalPlayer = player.IsLocal;

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

                DemonKingPanel.PlayerNameText.gameObject.SetActive(true);
                DemonKingPanel.PlayerNameText.text = player.PlayerNickName;

                DemonKingPanel.DemonKingScoreText.gameObject.SetActive(true);
                DemonKingPanel.DemonKingScoreText.text = player.DemonKingScore.ToString();

                DemonKingPanel.FillImg.gameObject.SetActive(true);
                DemonKingPanel.UpdateSliderValue(player.DemonKingScore);

                DemonKingPanel.CurrentEvolutionImg.gameObject.SetActive(true);
                DemonKingPanel.CurrentEvolutionImg.sprite = player.EvolutionImg.sprite;

                if (player.DemonKingScore >= DemonKingScoreRequiredToWin)
                    DidAWinOccur = true;
            }

            if (player.IslocalPlayer)
            {
                playerPositionOnScoreboard = i;
            }

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


    IEnumerator ReturnToLobbyCo()
    {
        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["IsSpectator"])
        {
            PlayerHealthManager health = GetComponent<PlayerHealthManager>();
            Devour devour = GetComponent<Devour>();

            if (health.beingDevoured)
                health.InterruptDevourOnSelf();
            if (devour.IsDevouring)
                devour.InteruptDevouring();

            transformViewClassic.m_PositionModel.TeleportIfDistanceGreaterThan = 0;
            podiumVCam.m_Priority = 15;
            playerController.DisableAllInputs();
            playerHUD.SetActive(false);
            playerNameOverhead.SetActive(true);
            demonKingVFX.SetActive(false);
            overheadNameTag.SetActive(true);

            GameManager.instance.GetComponent<EndGameLeaderboardManager>().DisplayEndgameLeaderboard(matchTime, evolutionManager.TimeAsSlimeTimer, evolutionManager.TimeAsLionTimer, evolutionManager.TimeAsLionKingTimer,
                evolutionManager.TimeAsRayTimer, evolutionManager.TimeAsRayKingTimer, evolutionManager.TimeAsDragonTimer, evolutionManager.TimeAsDragonKingTimer, playerController.SlimeDamageOutput, playerController.LionDamageOutput,
                playerController.RayDamageOutput, playerController.DragonDamageOutput);

            DemonKingInGameAnalytics.instance.SendEndGameStatsAnalytics();
            DemonKingInGameAnalytics.instance.SendEvolutionAnalytics();

            foreach (var player in players.items)
            {
                SpectatorCamera spec = player.GetComponent<SpectatorCamera>();
                if (spec == null)
                    player.GetComponent<PlayerHealthManager>().overheadHealthBar.gameObject.SetActive(false);
            }

            if (playerPositionOnScoreboard == 0)
            {
                playerController.transform.position = podium1.position;
                playerController.transform.rotation = podium1.transform.rotation;
            }
            else if (playerPositionOnScoreboard == 1)
            {
                playerController.transform.position = podium2.position;
                playerController.transform.rotation = podium2.transform.rotation;
            }
            else if (playerPositionOnScoreboard == 2)
            {
                playerController.transform.position = podium3.position;
                playerController.transform.rotation = podium3.transform.rotation;
            }
            else
            {
                playerController.transform.position = new Vector3(0, -200, 0);
            }
        }

        yield return new WaitForSeconds(30f);

        ChangeScene("Menu");
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
            PhotonNetwork.LoadLevel("Game");
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
