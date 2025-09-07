using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Range(0f, 1f)] public float masterVolume = 0.8f;
    [Range(0.05f, 5f)] public float defaultFadeSeconds = 1.5f;

    private AudioSource _a;
    private AudioSource _b;
    private AudioSource _active;    // currently audible source
    private Coroutine _xFadeCo;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _a = gameObject.AddComponent<AudioSource>();
        _b = gameObject.AddComponent<AudioSource>();
        foreach (var s in new[] { _a, _b })
        {
            s.loop = true;
            s.playOnAwake = false;
            s.spatialBlend = 0f; // 2D music
            s.volume = 0f;
        }
        _active = _a;

        // auto-track by scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Switch music based on scene name
        if (scene.name == "Dungeon")
        {
            PlayMusicByPath("Audio/Music/DungeonTheme", defaultFadeSeconds);
        }
        else if (scene.name == "Menu")
        {
            PlayMusicByPath("Audio/Music/MenuTheme", defaultFadeSeconds);
        }
        else if (scene.name == "Upgrade")
        {
            PlayMusicByPath("Audio/Music/UpgradeTheme", defaultFadeSeconds);
        }
        // add more scenes as needed
    }

    public void PlayMusicByPath(string resourcesPath, float fadeSeconds = -1f)
    {
        var clip = Resources.Load<AudioClip>(resourcesPath);
        if (clip == null) { Debug.LogWarning($"Music clip not found at Resources/{resourcesPath}"); return; }

        // If same clip already playing, do nothing
        if (_active.clip == clip && _active.isPlaying) return;

        if (_xFadeCo != null) StopCoroutine(_xFadeCo);
        _xFadeCo = StartCoroutine(CrossfadeTo(clip, fadeSeconds < 0 ? defaultFadeSeconds : fadeSeconds));
    }

    private IEnumerator CrossfadeTo(AudioClip next, float seconds)
    {
        AudioSource incoming = _active == _a ? _b : _a;
        AudioSource outgoing = _active;

        incoming.clip = next;
        incoming.time = 0f;
        incoming.volume = 0f;
        incoming.Play();

        float t = 0f;
        float outStart = outgoing.volume;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime; // unscaled so fades also work during pause
            float k = Mathf.Clamp01(t / seconds);
            incoming.volume = masterVolume * k;
            outgoing.volume = masterVolume * (1f - k) * (outStart / Mathf.Max(outStart, 0.0001f));
            yield return null;
        }

        // finalize
        incoming.volume = masterVolume;
        outgoing.Stop();
        outgoing.clip = null;
        outgoing.volume = 0f;
        _active = incoming;
        _xFadeCo = null;
    }

    public void SetVolume(float v)
    {
        masterVolume = Mathf.Clamp01(v);
        if (_active != null) _active.volume = masterVolume;
    }

    public void StopMusic(float fadeSeconds = 0.5f)
    {
        if (_xFadeCo != null) StopCoroutine(_xFadeCo);
        StartCoroutine(FadeOutAndStop(_active, fadeSeconds));
    }

    private IEnumerator FadeOutAndStop(AudioSource src, float seconds)
    {
        float start = src.volume;
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, 0f, t / seconds);
            yield return null;
        }
        src.Stop();
        src.clip = null;
        src.volume = 0f;
    }
}
