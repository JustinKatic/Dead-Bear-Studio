using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayWalkSound : MonoBehaviour
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
        footStepEvent.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void FootStepSound()
    {
        if (!IsPlaying(footStepEvent))
            footStepEvent.start();
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }
}
