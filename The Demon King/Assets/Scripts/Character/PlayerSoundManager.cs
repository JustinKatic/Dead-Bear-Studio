using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class PlayerSoundManager : MonoBehaviourPun
{
    [HideInInspector] public AnimationSounds animationSounds = new AnimationSounds();

    FMOD.Studio.EventInstance footStepEvent;
    FMOD.Studio.EventInstance fallingEvent;
    FMOD.Studio.EventInstance devourEvent;


    private bool createNewFallingInstance = true;

    private bool createNewDevourInstance = true;


    CharacterController cc;



    public static PlayerSoundManager Instance;

    private void Awake()
    {
        if (photonView.IsMine)
            Instance = this;
    }

    private void Start()
    {
        if (photonView.IsMine)
            cc = GetComponentInParent<CharacterController>();
    }


    private void Update()
    {
        //Update position of 3d sound instance
        footStepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        fallingEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    //Checks if instance is currently playing a sound already
    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    //Updates current sounds to match the current evolution
    public void ChangeCurrentEvolutionSounds(AnimationSounds newAnimationSounds)
    {
        animationSounds = newAnimationSounds;
        footStepEvent = FMODUnity.RuntimeManager.CreateInstance(animationSounds.WalkSound);
    }


    #region FootStep Sound
    public void FootStepSound()
    {
        if (photonView.IsMine)
        {
            if (!IsPlaying(footStepEvent) && cc.velocity.magnitude >= 0.2f)
            {
                footStepEvent.start();
                photonView.RPC("PlayFootStepSoundRPC", RpcTarget.Others, animationSounds.WalkSound);
            }
        }
    }

    [PunRPC]
    void PlayFootStepSoundRPC(string walkSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(walkSound, gameObject);
    }
    #endregion

    #region Jump Sound
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
    #endregion

    #region JumpLandNormal Sound
    public void PlayJumpLandNormalSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(animationSounds.LandingNormalSound, gameObject);
        photonView.RPC("PlayJumpLandNormalSoundRPC", RpcTarget.Others, animationSounds.LandingNormalSound);
    }

    [PunRPC]
    void PlayJumpLandNormalSoundRPC(string LandingNormalSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(LandingNormalSound, gameObject);
    }
    #endregion

    #region JumpLandBig Sound
    public void PlayJumpLandBigSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(animationSounds.LandingBigSound, gameObject);
        photonView.RPC("PlayJumpLandBigSoundRPC", RpcTarget.Others, animationSounds.LandingBigSound);
    }

    [PunRPC]
    void PlayJumpLandBigSoundRPC(string LandingBigSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(LandingBigSound, gameObject);
    }
    #endregion

    #region Falling Sound
    public void PlayFallingSound()
    {
        if (createNewFallingInstance)
        {
            createNewFallingInstance = false;
            fallingEvent = FMODUnity.RuntimeManager.CreateInstance(animationSounds.FallingSound);
            fallingEvent.start();
            photonView.RPC("PlayFallingSound_RPC", RpcTarget.Others, animationSounds.FallingSound);
        }
    }

    [PunRPC]
    void PlayFallingSound_RPC(string FallingSound)
    {
        fallingEvent = FMODUnity.RuntimeManager.CreateInstance(FallingSound);
        fallingEvent.start();
    }

    public void StopFallingSound()
    {
        createNewFallingInstance = true;
        fallingEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fallingEvent.release();
        photonView.RPC("StopFallingSound_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StopFallingSound_RPC()
    {
        fallingEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fallingEvent.release();
    }
    #endregion

    #region CastAbility1 Sound
    public void PlayAbility1Sound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(animationSounds.ShootSound, gameObject);
        //photonView.RPC("PlayAbility1Sound_RPC", RpcTarget.Others, animationSounds.ShootSound);
    }

    [PunRPC]
    void PlayAbility1Sound_RPC(string ShootSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(ShootSound, gameObject);
    }
    #endregion

    #region Devour Sound
    public void PlayDevourSound()
    {
        if (createNewDevourInstance)
        {
            createNewDevourInstance = false;
            devourEvent = FMODUnity.RuntimeManager.CreateInstance(animationSounds.DevourSound);
            devourEvent.start();
            photonView.RPC("PlayDevourSound_RPC", RpcTarget.Others, animationSounds.DevourSound);
        }
    }

    [PunRPC]
    void PlayDevourSound_RPC(string devourSound)
    {
        devourEvent = FMODUnity.RuntimeManager.CreateInstance(devourSound);
        devourEvent.start();
    }

    public void StopDevourSound()
    {
        createNewDevourInstance = true;
        devourEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        devourEvent.release();
        photonView.RPC("StopDevourSound_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StopDevourSound_RPC()
    {
        devourEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        devourEvent.release();
    }

    #endregion
}
