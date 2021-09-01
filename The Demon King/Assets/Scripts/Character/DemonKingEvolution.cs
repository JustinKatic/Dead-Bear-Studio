using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class DemonKingEvolution : MonoBehaviourPun
{
    [HideInInspector] public bool AmITheDemonKing = false;

    public float timeSpentAsDemonKing = 0;
    public int DemonKingScore;
    public IntSO DemonKingScoreRequiredToWin;

    public float ScaleAmount = 10;
    public GameObject DemonkingBeaconVFX;
    private EvolutionManager evolutionManager;
    private LeaderboardManager leaderboardManager;

    private bool hasPlayerWon = false;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            evolutionManager = GetComponent<EvolutionManager>();
            leaderboardManager = GetComponentInChildren<LeaderboardManager>();

            Hashtable DemonKingScoreHash = new Hashtable();
            DemonKingScoreHash.Add("DemonKingScore", DemonKingScore);
            PhotonNetwork.LocalPlayer.SetCustomProperties(DemonKingScoreHash);
        }
    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            if (!hasPlayerWon)
            {
                if (AmITheDemonKing)
                    timeSpentAsDemonKing += Time.deltaTime;

                if (DemonKingScore >= DemonKingScoreRequiredToWin.value)
                {
                    hasPlayerWon = true;

                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                    foreach (var player in players)
                    {
                        DisplayLeaderboardOnWin(player);
                    }
                }
            }
        }
    }

    public void UpdateDemonKingScore(int AmountToIncreaseScoreBy)
    {
        DemonKingScore += AmountToIncreaseScoreBy;
        Hashtable DemonKingScoreHash = new Hashtable();
        DemonKingScoreHash.Add("DemonKingScore", DemonKingScore);
        PhotonNetwork.LocalPlayer.SetCustomProperties(DemonKingScoreHash);
    }

    void DisplayLeaderboardOnWin(GameObject player)
    {
        player.GetPhotonView().RPC("DisplayLeaderboardOnWin_RPC", RpcTarget.All);
    }

    [PunRPC]
    void DisplayLeaderboardOnWin_RPC()
    {
        if (photonView.IsMine)
        {
            Invoke("DisplayEndGameLeaderboard", 1);
        }
    }
    void DisplayEndGameLeaderboard()
    {
        leaderboardManager.leaderboardEnabed = true;
        leaderboardManager.DidAWinOccur = true;
        leaderboardManager.DisplayLeaderboard();
    }

    public void ChangeToTheDemonKing()
    {
        if (photonView.IsMine)
        {
            AmITheDemonKing = true;
            AnnounceDemonKing();
            evolutionManager.ActivateDemonKingEvolution();
        }
    }



    public void AnnounceDemonKing()
    {
        photonView.RPC("AnnounceDemonKing_RPC", RpcTarget.Others);
    }

    [PunRPC]
    public void AnnounceDemonKing_RPC()
    {
        AmITheDemonKing = true;
        DemonkingBeaconVFX.SetActive(true);
    }

    public void KilledAsDemonKing()
    {
        photonView.RPC("KilledAsDemonKing_RPC", RpcTarget.All);
    }

    [PunRPC]
    public void KilledAsDemonKing_RPC()
    {
        AmITheDemonKing = false;
        if (!photonView.IsMine)
            DemonkingBeaconVFX.SetActive(false);
    }
}
