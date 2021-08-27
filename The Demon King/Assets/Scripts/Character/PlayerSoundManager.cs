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
    FMOD.Studio.EventInstance stunnedEvent;
    FMOD.Studio.EventInstance evolveEvent;
    FMOD.Studio.EventInstance DemonKingEvolveEvent;
    FMOD.Studio.EventInstance RayChargeUpEvent;


    private bool createNewFallingInstance = true;
    private bool UpdateFallingSoundPos = false;

    private bool createNewDevourInstance = true;
    private bool UpdateDevourSoundPos = false;

    private bool createNewStunnedInstance = true;
    private bool UpdateStunnedSoundPos = false;

    private bool createNewEvolveInstance = true;
    private bool UpdateEvolveSoundPos = false;

    private bool createNewDemonKingEvolveInstance = true;
    private bool UpdateDemonKingEvolveSoundPos = false;

    private bool createNewRayChargeUpEventInstance = true;
    private bool UpdateRayChargeUpEventSoundPos = false;


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
        if (photonView.IsMine)
            footStepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        //Only update position if a sound instance is in use
        if (UpdateFallingSoundPos)
            fallingEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        if (UpdateDevourSoundPos)
            devourEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        if (UpdateStunnedSoundPos)
            stunnedEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        if (UpdateEvolveSoundPos)
            evolveEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        if (UpdateDemonKingEvolveSoundPos)
            DemonKingEvolveEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        if (UpdateRayChargeUpEventSoundPos)
            RayChargeUpEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
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

    #region Death By Lava Sound
    public void PlayDeathByLavaSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(animationSounds.DeathByLavaSound, gameObject);
        photonView.RPC("PlayDeathByLavaSoundOneShot", RpcTarget.Others, animationSounds.DeathByLavaSound);
    }

    [PunRPC]
    void PlayDeathByLavaSoundOneShot(string DeathByLavaSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(DeathByLavaSound, gameObject);
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
            UpdateFallingSoundPos = true;
            photonView.RPC("PlayFallingSound_RPC", RpcTarget.Others, animationSounds.FallingSound);

        }
    }

    [PunRPC]
    void PlayFallingSound_RPC(string FallingSound)
    {
        fallingEvent = FMODUnity.RuntimeManager.CreateInstance(FallingSound);
        fallingEvent.start();
        UpdateFallingSoundPos = true;
    }

    public void StopFallingSound()
    {
        createNewFallingInstance = true;
        UpdateFallingSoundPos = false;
        fallingEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fallingEvent.release();
        photonView.RPC("StopFallingSound_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StopFallingSound_RPC()
    {
        fallingEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fallingEvent.release();
        UpdateFallingSoundPos = false;

    }
    #endregion

    #region CastAbility1 Sound
    public void PlayCastAbilitySound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(animationSounds.ShootSound, gameObject);
        photonView.RPC("PlayAbility1Sound_RPC", RpcTarget.Others, animationSounds.ShootSound);
    }

    [PunRPC]
    void PlayAbility1Sound_RPC(string ShootSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(ShootSound, gameObject);
    }
    #endregion

    #region RayFullyChargedUpShootSound
    public void PlayRayFullyChargedUpShootSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(animationSounds.RayFullyChargedUpShootSound, gameObject);
        photonView.RPC("PlayRayFullyChargedUpShootSound_RPC", RpcTarget.Others, animationSounds.RayFullyChargedUpShootSound);
    }

    [PunRPC]
    void PlayRayFullyChargedUpShootSound_RPC(string ShootSound)
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
            UpdateDevourSoundPos = true;
            photonView.RPC("PlayDevourSound_RPC", RpcTarget.Others, animationSounds.DevourSound);
        }
    }

    [PunRPC]
    void PlayDevourSound_RPC(string devourSound)
    {
        devourEvent = FMODUnity.RuntimeManager.CreateInstance(devourSound);
        devourEvent.start();
        UpdateDevourSoundPos = true;
    }

    public void StopDevourSound()
    {
        createNewDevourInstance = true;
        devourEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        devourEvent.release();
        UpdateDevourSoundPos = false;
        photonView.RPC("StopDevourSound_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StopDevourSound_RPC()
    {
        devourEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        devourEvent.release();
        UpdateDevourSoundPos = false;
    }
    #endregion

    #region Stunned Sound
    public void PlayStunnedSound()
    {
        if (createNewStunnedInstance)
        {
            createNewStunnedInstance = false;
            stunnedEvent = FMODUnity.RuntimeManager.CreateInstance(animationSounds.StunSound);
            stunnedEvent.start();
            UpdateStunnedSoundPos = true;
            photonView.RPC("PlayStunnedSound_RPC", RpcTarget.Others, animationSounds.StunSound);
        }
    }

    [PunRPC]
    void PlayStunnedSound_RPC(string stunnedSound)
    {
        stunnedEvent = FMODUnity.RuntimeManager.CreateInstance(stunnedSound);
        stunnedEvent.start();
        UpdateStunnedSoundPos = true;
    }

    public void StopStunnedSound()
    {
        createNewStunnedInstance = true;
        stunnedEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        stunnedEvent.release();
        UpdateStunnedSoundPos = false;
        photonView.RPC("StopStunnedSound_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StopStunnedSound_RPC()
    {
        stunnedEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        stunnedEvent.release();
        UpdateStunnedSoundPos = false;
    }

    #endregion

    #region NormalEvolveSound
    public void PlayEvolveSound()
    {
        if (createNewEvolveInstance)
        {
            createNewEvolveInstance = false;
            evolveEvent = FMODUnity.RuntimeManager.CreateInstance(animationSounds.EvolveSound);
            evolveEvent.start();
            UpdateEvolveSoundPos = true;
            photonView.RPC("PlayEvolveSound_RPC", RpcTarget.Others, animationSounds.EvolveSound);
        }
    }

    [PunRPC]
    void PlayEvolveSound_RPC(string evolveSound)
    {
        evolveEvent = FMODUnity.RuntimeManager.CreateInstance(evolveSound);
        evolveEvent.start();
        UpdateEvolveSoundPos = true;
    }

    public void StopEvolveSound()
    {
        createNewEvolveInstance = true;
        evolveEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        evolveEvent.release();
        UpdateEvolveSoundPos = false;
        photonView.RPC("StopEvolveSound_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StopEvolveSound_RPC()
    {
        evolveEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        evolveEvent.release();
        UpdateEvolveSoundPos = false;
    }

    #endregion

    #region DemonKingEvolve Sound
    public void PlayDemonKingEvolveSound()
    {
        if (createNewDemonKingEvolveInstance)
        {
            createNewDemonKingEvolveInstance = false;
            DemonKingEvolveEvent = FMODUnity.RuntimeManager.CreateInstance(animationSounds.DemonKingEvolveSound);
            DemonKingEvolveEvent.start();
            UpdateDemonKingEvolveSoundPos = true;
            photonView.RPC("PlayDemonKingEvolveSound_RPC", RpcTarget.Others, animationSounds.DemonKingEvolveSound);
        }
    }

    [PunRPC]
    void PlayDemonKingEvolveSound_RPC(string DemonKingEvolveSound)
    {
        DemonKingEvolveEvent = FMODUnity.RuntimeManager.CreateInstance(DemonKingEvolveSound);
        DemonKingEvolveEvent.start();
        UpdateDemonKingEvolveSoundPos = true;
    }

    public void StopDemonKingEvolveSound()
    {
        createNewDemonKingEvolveInstance = true;
        DemonKingEvolveEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        DemonKingEvolveEvent.release();
        UpdateDemonKingEvolveSoundPos = false;
        photonView.RPC("StopDemonKingEvolveSound_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StopDemonKingEvolveSound_RPC()
    {
        DemonKingEvolveEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        DemonKingEvolveEvent.release();
        UpdateDemonKingEvolveSoundPos = false;
    }

    #endregion

    #region DemonKingAnnouncement Sound
    public void PlayDemonKingAnnouncementSound()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(animationSounds.DemonKingAnnouncementSound, gameObject);
        photonView.RPC("PlayDemonKingAnnouncementSound_RPC", RpcTarget.Others, animationSounds.DemonKingAnnouncementSound);
    }

    [PunRPC]
    void PlayDemonKingAnnouncementSound_RPC(string jumpSound)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(jumpSound, gameObject);
    }

    #endregion

    #region RayChargeUpEvent
    public void PlayRayChargeUpSound()
    {
        if (createNewRayChargeUpEventInstance)
        {
            createNewRayChargeUpEventInstance = false;
            RayChargeUpEvent = FMODUnity.RuntimeManager.CreateInstance(animationSounds.RayChargeUpSound);
            RayChargeUpEvent.start();
            UpdateRayChargeUpEventSoundPos = true;
            photonView.RPC("PlayRayChargeUpSound_RPC", RpcTarget.Others, animationSounds.RayChargeUpSound);
        }
    }

    [PunRPC]
    void PlayRayChargeUpSound_RPC(string RayChargeUpSound)
    {
        RayChargeUpEvent = FMODUnity.RuntimeManager.CreateInstance(RayChargeUpSound);
        RayChargeUpEvent.start();
        UpdateRayChargeUpEventSoundPos = true;
    }

    public void StopRayChargeUpSound()
    {
        createNewRayChargeUpEventInstance = true;
        RayChargeUpEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        RayChargeUpEvent.release();
        UpdateRayChargeUpEventSoundPos = false;
        photonView.RPC("StopRayChargeUpSound_RPC", RpcTarget.Others);
    }

    [PunRPC]
    void StopRayChargeUpSound_RPC()
    {
        RayChargeUpEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        RayChargeUpEvent.release();
        UpdateRayChargeUpEventSoundPos = false;
    }
    #endregion
}
