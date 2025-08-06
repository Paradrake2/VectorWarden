using TMPro;
using UnityEngine;

public class StatUpgradeButton : MonoBehaviour
{
    public PlayerStats playerStats;
    public StatType upgradeStatType;
    public TextMeshProUGUI costText;
    public float upgradeAmount = 1f;
    public float baseUpgradeCost = 10f;
    public int timesBought = 0;
    public float CalculateCost()
    {
        float cost;
        return cost = baseUpgradeCost * Mathf.Pow(1.2f, timesBought);
    }
    public void UpgradeStat()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is not assigned.");
            return;
        }

        if (UpgradeManager.Instance.GetUpgradeXPAmount() < CalculateCost())
        {
            Debug.LogWarning("Not enough XP to upgrade.");
            float neededXP = CalculateCost() - UpgradeManager.Instance.GetUpgradeXPAmount();
            UpgradeUIManager.Instance.ShowNotEnoughXPPanel(neededXP);
            return;
        }
        switch (upgradeStatType)
        {
            case StatType.MaxHealth:
                playerStats.BaseHealth += upgradeAmount;
                break;
            case StatType.Damage:
                playerStats.BaseDamage += upgradeAmount;
                break;
            case StatType.Defense:
                playerStats.BaseDefense += upgradeAmount;
                break;
            case StatType.PlayerMoveSpeed:
                playerStats.BaseMoveSpeed += upgradeAmount;
                break;
            case StatType.Shield:
                playerStats.BaseShield += upgradeAmount;
                break;
            case StatType.XPGain:
                playerStats.BaseXPGain += upgradeAmount;
                break;
            default:
                Debug.LogWarning("Unknown stat type for upgrade.");
                break;
        }
        timesBought++;
        UpgradeManager.Instance.RemoveXP(CalculateCost());
        costText.text = CalculateCost().ToString();
        // UIManager.Instance.UpdateHealthText();
        // UIManager.Instance.UpdateShieldText();
        // UIManager.Instance.UpdateDamageText();
        // UIManager.Instance.UpdateDefenseText();
    }
    void Start()
    {
        playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats instance not found.");
        }
        costText.text = CalculateCost().ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
