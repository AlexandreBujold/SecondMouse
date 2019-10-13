using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // Singleton Declaration
    public static AudioManager instance = null;

    // Declare fmod busses.
    FMOD.Studio.Bus Master;
    FMOD.Studio.Bus SoundEffects;
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus Ambient;

    public float score;
    public TMPro.TextMeshProUGUI scoreText;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += GetReferences;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= GetReferences;
    }

    void Update()
    {
        // Set fmod buss volumes to equal script volume control variables.
        Master.setVolume(masterVolume);
        SoundEffects.setVolume(soundEffectsVolume);
        Music.setVolume(musicVolume);
        Ambient.setVolume(ambientVolume);

        if (scoreText != null)
        {
            scoreText.text = "Final Score: " + score;
        }
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

    public void GetReferences(Scene scene, LoadSceneMode mode)
    {
        scoreText = GameObject.Find("Score Value").GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    public void GoToStart()
    {
        SceneManager.LoadScene(0);
    }


}
