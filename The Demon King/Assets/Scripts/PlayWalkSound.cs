using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayWalkSound : MonoBehaviourPun
{
    [FMODUnity.EventRef]
    public string WalkSound;

    FMOD.Studio.EventInstance footStepEvent;

    CharacterController cc;




    private void Start()
    {
        cc = GetComponentInParent<CharacterController>();
        footStepEvent = FMODUnity.RuntimeManager.CreateInstance(WalkSound);
    }

    private void Update()
    {
        footStepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void FootStepSound()
    {
        if (photonView.IsMine)
        {
            if (!IsPlaying(footStepEvent) && cc.velocity.magnitude >= 0.2f)
            {
                
                footStepEvent.start();
                photonView.RPC("PlayWalkSoundRPC", RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    void PlayWalkSoundRPC()
    {
        footStepEvent.start();
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}
