using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class DemonKingEvolution : MonoBehaviourPun
{
    [HideInInspector] public bool AmITheDemonKing = false;

    public float timeSpentAsDemonKing = 0;
    public float TimeRequiredToWin = 10;
    private ExperienceManager experienceManager;
    private LeaderboardManager leaderboardManager;

    private bool hasPlayerWon = false;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            experienceManager = GetComponent<ExperienceManager>();
            leaderboardManager = GetComponentInChildren<LeaderboardManager>();

            Hashtable hash = new Hashtable();
            hash.Add("TimeAsDemonKing", timeSpentAsDemonKing);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (AmITheDemonKing)
            {
                timeSpentAsDemonKing += Time.deltaTime;
                Hashtable hash = new Hashtable();
                hash.Add("TimeAsDemonKing", timeSpentAsDemonKing);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
            if (!hasPlayerWon && timeSpentAsDemonKing >= TimeRequiredToWin)
            {
                hasPlayerWon = true;
                leaderboardManager.DisplayLeaderboard(true);
            }
        }
    }



    public void ChangeToTheDemonKing()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("AnnounceDemonKing", RpcTarget.All);
            experienceManager.ActivateDemonKingEvolution();
        }

    }
    public void ChangeFromTheDemonKing()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("DevouredAsDemonKing", RpcTarget.All);
        }

    }

    [PunRPC]
    public void AnnounceDemonKing()
    {
        AmITheDemonKing = true;
    }

    [PunRPC]
    public void DevouredAsDemonKing()
    {
        AmITheDemonKing = false;
    }
}
