using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // instance
    public static NetworkManager instance;

    private PhotonView PV;
    private PhotonVoiceView VV;
    private
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            if (PV == null)
            {
                PV = gameObject.AddComponent<PhotonView>();
                photonView.ViewID = 999;
                VV = gameObject.AddComponent<PhotonVoiceView>();
                VV.RecorderInUse = GetComponent<Recorder>();
                VV.SpeakerInUse = GetComponent<Speaker>();
            }
        }
        DontDestroyOnLoad(gameObject);
    }



    // changes the scene through Photon's system
    [PunRPC]
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

    // called when we disconnect from the Photon server
    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("A Player left room");
        if (PhotonNetwork.IsMasterClient && GameManager.instance != null)
        {
            bool IsThereAKing = false;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (GameManager.instance.GetPlayer((int)player.CustomProperties["PlayerId"]).GetComponent<DemonKingEvolution>().AmITheDemonKing)
                {
                    IsThereAKing = true;
                }

            }
            if (!IsThereAKing)
                GameObject.FindGameObjectWithTag("DemonKingCrown").GetComponent<CrownHealthManager>().CrownRespawn(true);
        }
    }
}