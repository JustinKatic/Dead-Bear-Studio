using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySoundManager : MonoBehaviourPun
{
    [FMODUnity.EventRef]
    public string WalkSound;

    FMOD.Studio.EventInstance footStepEvent;


    private void Start()
    {
        footStepEvent = FMODUnity.RuntimeManager.CreateInstance(WalkSound);
    }

    private void Update()
    {
        //Update position of 3d sound instance
        if (photonView.IsMine)
            footStepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void PlayFootStepSound()
    {
        if (photonView.IsMine)
        {
            if (!IsPlaying(footStepEvent))
            {
                Debug.Log("Enemy footstep");
                footStepEvent.start();
                photonView.RPC("PlayEnemyFootStepSoundRPC", RpcTarget.Others, WalkSound);
            }
        }
    }

    [PunRPC]
    void PlayEnemyFootStepSoundRPC(string walkSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(walkSound, gameObject);
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}
