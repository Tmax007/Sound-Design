using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private EventInstance currentMusicInstance;
    private EventInstance currentAmbienceInstance;

    private string currentMusicEvent = "";
    private string currentAmbienceEvent = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllAudio();

        if (scene.name == "MainMenu")
        {
            PlayMusic("event:/Music/Menu");
        }
    }

    public void PlayMusic(string musicEvent, float fadeDuration = 1f)
    {
        if (string.IsNullOrEmpty(musicEvent))
        {
            StartCoroutine(FadeOutAndStop(currentMusicInstance, (ei) => currentMusicInstance = ei));
            currentMusicEvent = "";
            return;
        }

        if (musicEvent == currentMusicEvent) return;

        currentMusicEvent = musicEvent;
        StartCoroutine(SwapEvent(musicEvent, fadeDuration, currentMusicInstance, (ei) => currentMusicInstance = ei));
    }

    public void PlayAmbience(string ambienceEvent, float fadeDuration = 1f)
    {
        if (string.IsNullOrEmpty(ambienceEvent))
        {
            StartCoroutine(FadeOutAndStop(currentAmbienceInstance, (ei) => currentAmbienceInstance = ei));
            currentAmbienceEvent = "";
            return;
        }

        if (ambienceEvent == currentAmbienceEvent) return;

        currentAmbienceEvent = ambienceEvent;
        StartCoroutine(SwapEvent(ambienceEvent, fadeDuration, currentAmbienceInstance, (ei) => currentAmbienceInstance = ei));
    }

    private IEnumerator SwapEvent(string newEvent, float fadeDuration, EventInstance oldInstance, System.Action<EventInstance> assignInstance)
    {
        yield return FadeOutAndStop(oldInstance, assignInstance);
        EventInstance newInstance = RuntimeManager.CreateInstance(newEvent);
        assignInstance(newInstance);
        yield return FadeIn(newInstance, fadeDuration);
    }

    private IEnumerator FadeOutAndStop(EventInstance instance, System.Action<EventInstance> assignNull, float duration = 1f)
    {
        if (!instance.isValid())
        {
            assignNull(default);
            yield break;
        }

        instance.getVolume(out float startVolume);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            instance.setVolume(Mathf.Lerp(startVolume, 0f, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        instance.setVolume(0f);
        instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        bool playing = true;
        while (playing)
        {
            instance.getPlaybackState(out var state);
            if (state == PLAYBACK_STATE.STOPPED) playing = false;
            yield return null;
        }

        instance.release();
        assignNull(default);
    }

    private IEnumerator FadeIn(EventInstance instance, float duration = 1f)
    {
        instance.setVolume(0f);
        instance.start();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            instance.setVolume(Mathf.Lerp(0f, 1f, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        instance.setVolume(1f);
    }

    public void StopAllAudio()
    {
        StartCoroutine(FadeOutAndStop(currentMusicInstance, (ei) => currentMusicInstance = ei));
        currentMusicEvent = "";

        StartCoroutine(FadeOutAndStop(currentAmbienceInstance, (ei) => currentAmbienceInstance = ei));
        currentAmbienceEvent = "";
    }
}