using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayerStats playerStats;
    public TextMeshProUGUI healthText;
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
    }
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found in the scene.");
        }
    }
    public void UpdateHealthText()
    {
        if (healthText != null && playerStats != null)
        {
            healthText.text = $"Health: {playerStats.CurrentHealth}/{playerStats.CurrentMaxHealth}";
        }
        else
        {
            Debug.LogWarning("Health text or PlayerStats is not set.");
        }
    }   
    void Update()
    {
        
    }
}
