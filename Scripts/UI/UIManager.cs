using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public PlayerStats playerStats;
    public Player player;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI defenseText;
    public Transform pauseMenu;
    public GameObject bossHealthBar;
    void Awake()
    {
        Instance = this;
    }
    public void BindPlayer(Player p) {
        player = p;
        playerStats = PlayerStats.Instance; // your stats are DDOL
        bossHealthBar.SetActive(false);     // ensure correct default on each bind
        UpdateHealthText(); UpdateShieldText(); /* etc. */
    }
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats not found in the scene.");
        }
        player = FindFirstObjectByType<Player>();
        bossHealthBar.SetActive(false);
        if (player == null)
        {
            Debug.LogError("Cant find player");
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

    public void UpdateShieldText()
    {
        if (shieldText != null && playerStats != null)
        {
            shieldText.text = $"Shield: {playerStats.CurrentShield}/{playerStats.CalculateMaxShield}";
        }
        else
        {
            Debug.LogWarning("Shield text or PlayerStats is not set.");
        }
    }
    public void UpdateDamageText()
    {
        if (damageText != null && playerStats != null)
        {
            damageText.text = $"Damage: {playerStats.CurrentDamage}";
        }
        else
        {
            Debug.LogWarning("Damage text or PlayerStats is not set.");
        }
    }
    public void UpdateDefenseText()
    {
        if (defenseText != null && playerStats != null)
        {
            defenseText.text = $"Defense: {playerStats.CurrentDefense}";
        }
        else
        {
            Debug.LogWarning("Defense text or PlayerStats is not set.");
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
    public void ShowBossHealthbar()
    {
        bossHealthBar.SetActive(true);
    }
    public void HideBossHealthbar()
    {
        bossHealthBar.SetActive(false);
    }
    public void ToRefineMenu()
    {
        SceneManager.LoadScene("Refine");
    }
    void Update()
    {

    }
    public void KillPlayer()
    {
        player.TakeDamage(999999999999999, "command");
    }
}
