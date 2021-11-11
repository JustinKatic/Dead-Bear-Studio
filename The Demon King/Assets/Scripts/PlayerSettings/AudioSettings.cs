using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    // Start is called before the first frame update
    private FMOD.Studio.Bus Master;
    private FMOD.Studio.Bus Music;
    private FMOD.Studio.Bus SFX;

    private float MasterVolume = 1f;
    private float SFXVolume = 1f;
    private float MusicVolume = 1f;

    public Slider SFXSlider;
    public Slider MusicSlider;
    public Slider MasterSlider;

    void Start()
    {
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/Master/SFX");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");

        SFXSlider.maxValue = 1;
        MusicSlider.maxValue = 1;
        MasterSlider.maxValue = 1;
        
        SFXSlider.value = SFXVolume;
        MusicSlider.value = MusicVolume;
        MasterSlider.value = MasterVolume;
        
        SFXSlider.onValueChanged.AddListener(SFXVolumeLevel);
        MasterSlider.onValueChanged.AddListener(MasterVolumeLevel);
        MusicSlider.onValueChanged.AddListener(MusicVolumeLevel);
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
