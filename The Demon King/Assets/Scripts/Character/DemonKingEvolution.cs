using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DemonKingEvolution : MonoBehaviourPun
{
    [HideInInspector] public bool CanEvolveToDemonKing = true;
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
            AmITheDemonKing = true;
            photonView.RPC("AnnounceDemonKing", RpcTarget.All);
            experienceManager.ActivateDemonKingEvolution();
        }
        
    }

    [PunRPC]
    public void AnnounceDemonKing()
    {
        // Tell everyone they Can,t evolve to the Demon King
        if (!photonView.IsMine)
        {
            CanEvolveToDemonKing = false;
        }

    }
    
    
}
