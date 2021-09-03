using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    private FMOD.Studio.EventInstance VolumeSettings;
    // Start is called before the first frame update
    private FMOD.Studio.Bus Master;
    private FMOD.Studio.Bus Music;
    private FMOD.Studio.Bus SFX;

    private float MasterVolume = 1f;
    private float SFXVolume = 0.5f;
    private float MusicVolume = 0.5f;

    void Awake()
    {
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
    }

    // Update is called once per frame
    void Update()
    {
        Music.setVolume(MusicVolume);
        SFX.setVolume(SFXVolume);
        Master.setVolume(MasterVolume);
    }

    public void SFXVolumeLevel(float newSFXVolume)
    {
        SFXVolume = newSFXVolume;
    }
    public void MasterVolumeLevel(float newMasterVolume)
    {
        MasterVolume = newMasterVolume;
    }    
    public void MusicVolumeLevel(float newMusicVolume)
    {
        MusicVolume = newMusicVolume;
    }
}
