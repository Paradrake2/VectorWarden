using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuControls : MonoBehaviour
{
    public GameObject volumeText;
    public MusicManager musicManager;
    public Slider volumeSlider;
    public void SetVolume()
    {
        if (musicManager == null)
        {
            musicManager = MusicManager.Instance ?? FindFirstObjectByType<MusicManager>();
            if (musicManager == null) { Debug.LogError("No MusicManager found."); return; }
        }
        musicManager.SetVolume(volumeSlider.value);
        volumeText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Volume: {(int)(musicManager.masterVolume * 100)}%";
    }

    void Start()
    {
        musicManager = MusicManager.Instance ?? FindFirstObjectByType<MusicManager>();
        volumeSlider.value = musicManager.masterVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
