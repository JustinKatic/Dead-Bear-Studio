using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class DemonKingEvolution : MonoBehaviourPun
{
    [HideInInspector] public bool AmITheDemonKing = false;

    public float timeSpentAsDemonKing = 0;


    public float ScaleAmount = 10;
    public GameObject DemonkingBeaconVFX;
    private EvolutionManager evolutionManager;


    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            evolutionManager = GetComponent<EvolutionManager>();
        }
    }


    private void Update()
    {
        if (photonView.IsMine)
        {
            if (AmITheDemonKing)
                timeSpentAsDemonKing += Time.deltaTime;
        }
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
