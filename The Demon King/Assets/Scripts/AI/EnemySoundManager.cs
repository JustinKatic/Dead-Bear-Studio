using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySoundManager : MonoBehaviourPun
{
    [FMODUnity.EventRef]
    public string WalkSound;

    [FMODUnity.EventRef]
    public string SlashAttackSound;

    [FMODUnity.EventRef]
    public string ChompAttackSound;

    FMOD.Studio.EventInstance footStepEvent;


    private void Start()
    {
        footStepEvent = FMODUnity.RuntimeManager.CreateInstance(WalkSound);
    }

    private void Update()
    {
        footStepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void PlayFootStepSound()
    {
        if (!IsPlaying(footStepEvent))
        {
            footStepEvent.start();
        }
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }


    #region Slash Attack Sounds
    public void PlaySlashAttackSound()
    {
        photonView.RPC("PlaySlashAttackSound_RPC", RpcTarget.All, SlashAttackSound);
    }

    [PunRPC]
    void PlaySlashAttackSound_RPC(string attackSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(attackSound, gameObject);
    }
    #endregion

    #region Chomp Attack Sound
    public void PlayChompAttackSound()
    {
        photonView.RPC("PlayChompAttackSound_RPC", RpcTarget.All, ChompAttackSound);
    }

    [PunRPC]
    void PlayChompAttackSound_RPC(string attackSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(attackSound, gameObject);
    }
    #endregion
}
