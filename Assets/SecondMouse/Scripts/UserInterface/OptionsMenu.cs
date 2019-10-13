using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    //
    //FMOD.Studio.EventInstance VolumeTestEvent;

    // Declare fmod busses.
    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus SoundEffects;
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus Ambient;

    // Declare & initialize volume variables.
    float masterVolume = 1f;
    float soundEffectsVolume = 1f;
    float musicVolume = 1f;
    float ambientVolume = 1f;

    public void Awake()
    {
        // Initialize fmod busses.
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        SoundEffects = FMODUnity.RuntimeManager.GetBus("bus:/Master/SoundFX");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        Ambient = FMODUnity.RuntimeManager.GetBus("bus:/Master/Ambient");

        // Initalize audio test event
        //VolumeTestEvent = FMODUnity.RuntimeManager.CreateInstance("event:/SoundFX/CollectItem");
        //VolumeTestEvent.start();
    }

    public void Update()
    {
        // Set fmod buss volumes to equal script volume control variables.
        Master.setVolume(masterVolume);
        SoundEffects.setVolume(soundEffectsVolume);
        Music.setVolume(musicVolume);
        Ambient.setVolume(ambientVolume);
    }

    public void SetMasterVolume(float volumeLevel)
    {
        masterVolume = volumeLevel;
    }

    public void SetSoundEffectsVolume(float volumeLevel)
    {
        soundEffectsVolume = volumeLevel;
    }

    public void SetMusicVolume(float volumeLevel)
    {
        musicVolume = volumeLevel;
    }

    public void SetAmbientVolume(float volumeLevel)
    {
        ambientVolume = volumeLevel;
    }
}
