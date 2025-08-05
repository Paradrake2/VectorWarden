using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPUIManager : MonoBehaviour
{
    public static XPUIManager Instance;
    public PlayerStats stats;
    public Image xpBar;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI levelText;
    void Start()
    {
        Instance = this;
        stats = PlayerStats.Instance;
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void UpdateLevelText()
    {
        levelText.text = stats.Level.ToString();
    }
    public void UpdateXPBarFill()
    {
        xpBar.fillAmount = stats.XP / stats.XpToNextLevel;
    }
    public void UpdateXPText()
    {
        xpText.text = $"{stats.XP}/{stats.XpToNextLevel}";
    }
}
