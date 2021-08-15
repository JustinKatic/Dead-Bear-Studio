using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class PlayerSoundManager : MonoBehaviourPun
{
    [HideInInspector] public AnimationSounds animationSounds = new AnimationSounds();

    FMOD.Studio.EventInstance footStepEvent;

    CharacterController cc;
    private void Start()
    {
        if (photonView.IsMine)
            cc = GetComponentInParent<CharacterController>();
    }

    private void Update()
    {
        footStepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void FootStepSound()
    {
        if (photonView.IsMine)
        {
            if (!IsPlaying(footStepEvent) && cc.velocity.magnitude >= 0.2f)
            {
                footStepEvent.start();
                photonView.RPC("PlayWalkSoundRPC", RpcTarget.Others, animationSounds.WalkSound);
            }
        }
    }

    [PunRPC]
    void PlayWalkSoundRPC(string walkSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(walkSound, gameObject);
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    public void ChangeEvolutionMovementSound(AnimationSounds newAnimationSounds)
    {
        animationSounds = newAnimationSounds;
        footStepEvent = FMODUnity.RuntimeManager.CreateInstance(animationSounds.WalkSound);
    }

    public void PlayJumpSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(animationSounds.JumpSound, gameObject);
        photonView.RPC("PlayJumpOneShot", RpcTarget.Others, animationSounds.JumpSound);
    }


    [PunRPC]
    void PlayJumpOneShot(string jumpSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(jumpSound, gameObject);
    }


}
