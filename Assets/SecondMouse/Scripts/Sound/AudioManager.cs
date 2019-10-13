using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton Declaration
    public static AudioManager instance = null;

    // Declare fmod busses.
    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus SoundEffects;
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus Ambient;

    // Declare & initialize volume variables.
    float masterVolume = 1f;
    float soundEffectsVolume = 1f;
    float musicVolume = 1f;
    float ambientVolume = 0.25f;

    void Awake()
    {
        // Singleton Initialization
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        // Initialize fmod busses.
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        SoundEffects = FMODUnity.RuntimeManager.GetBus("bus:/Master/SoundFX");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Master/Music");
        Ambient = FMODUnity.RuntimeManager.GetBus("bus:/Master/Ambient");
    }

    void Update()
    {
        // Set fmod buss volumes to equal script volume control variables.
        Master.setVolume(masterVolume);
        SoundEffects.setVolume(soundEffectsVolume);
        Music.setVolume(musicVolume);
        Ambient.setVolume(ambientVolume);
    }

    // Volume slider control
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
