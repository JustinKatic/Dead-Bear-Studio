using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DemonKingEvolution : MonoBehaviourPun
{
    [HideInInspector] public bool AmITheDemonKing = false;

    private ExperienceManager experienceManager;

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            experienceManager = GetComponent<ExperienceManager>();
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
            experienceManager.DecreaseExperince(experienceManager.DemonKingExpLossDeath);
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
