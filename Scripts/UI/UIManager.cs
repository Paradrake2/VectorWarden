using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayerStats playerStats;
    public TextMeshProUGUI healthText;
    public Transform pauseMenu;
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

    public void TogglePauseMenu()
    {
        if (pauseMenu != null)
        {
            bool isActive = pauseMenu.gameObject.activeSelf;
            pauseMenu.gameObject.SetActive(!isActive);
            Time.timeScale = isActive ? 1 : 0; // Pause or resume the game
        }
        else
        {
            Debug.LogWarning("Pause menu not assigned in UIManager.");
        }
    }

    public void ResumeGame()
    {
        if (pauseMenu != null)
        {
            pauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1; // Resume the game
        }
        else
        {
            Debug.LogWarning("Pause menu not assigned in UIManager.");
        }
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting...");
    }

    public void ToRefineMenu()
    {
        SceneManager.LoadScene("Refine");
    }
    void Update()
    {
        
    }
}
