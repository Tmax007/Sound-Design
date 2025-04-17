using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [EventRef] public string musicEventPath = "event:/Music/Music_Manager";
    [EventRef] public string ambienceEventPath = "event:/Ambience/Ambience_Manager";

    private EventInstance musicParameterInstance;
    private EventInstance ambienceParameterInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        InitParameterInstances();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            SetMusicState("MainMenu");
            SetAmbienceState("Default");
        }
    }

    private void InitParameterInstances()
    {
        musicParameterInstance = RuntimeManager.CreateInstance(musicEventPath);
        musicParameterInstance.start();

        ambienceParameterInstance = RuntimeManager.CreateInstance(ambienceEventPath);
        ambienceParameterInstance.start();
    }

    public void SetMusicState(string label)
    {
        if (musicParameterInstance.isValid() && !string.IsNullOrEmpty(label))
        {
            musicParameterInstance.setParameterByNameWithLabel("MusicState", label);
        }
    }

    public void SetAmbienceState(string label)
    {
        if (ambienceParameterInstance.isValid() && !string.IsNullOrEmpty(label))
        {
            ambienceParameterInstance.setParameterByNameWithLabel("AmbienceState", label);
        }
    }

    public void StopAllAudio()
    {
        if (musicParameterInstance.isValid())
        {
            musicParameterInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            musicParameterInstance.release();
        }
        if (ambienceParameterInstance.isValid())
        {
            ambienceParameterInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            ambienceParameterInstance.release();
        }
    }
}
