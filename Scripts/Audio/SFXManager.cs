using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    public AudioSource audioSource;
    public int maxSimultaneousSounds = 4;
    [Range(0f, 0.2f)] public float pitchVariation = 0.01f;
    public float minInterval = 0.01f;

    int playingNow = 0;
    float lastTime = 0f;

    void Start()
    {
        Instance = this;
    }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        audioSource.spatialBlend = 0f;
        audioSource.dopplerLevel = 0f;
    }

    public void PlaySound(AudioClip clip)
    {

        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void PlayPickupSound(AudioClip clip, float volume)
    {
        if (Time.time - lastTime < minInterval) return;
        if (playingNow >= maxSimultaneousSounds) return;

        lastTime = Time.time;
        playingNow++;

        float oldPitch = audioSource.pitch;
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(clip, volume);
        Instance.Invoke(nameof(ResetAfterOneShot), clip.length / audioSource.pitch);
    }
    void ResetAfterOneShot()
    {
        playingNow = Mathf.Max(0, playingNow - 1);
        audioSource.pitch = 1f;
    }
}
