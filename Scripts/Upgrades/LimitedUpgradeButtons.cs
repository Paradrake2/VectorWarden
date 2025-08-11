using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LimitedUpgradeButtons : MonoBehaviour
{
    public List<float> upgradeCosts = new List<float>(); // manually set upgrade costs in inspector
    public int MaxUpgrades = 5;
    public TextMeshProUGUI upgradeCostText;
    float amountToUpgrade = 1; // amount to increase
    private int currentUpgradeLevel = 0;
    

    public void UpgradeCardOptions()
    {
        if (currentUpgradeLevel >= MaxUpgrades)
        {
            Debug.LogWarning("Maximum upgrade level reached.");
            return;
        }
        if (UpgradeManager.Instance.GetUpgradeXPAmount() >= upgradeCosts[currentUpgradeLevel])
        {
            UpgradeManager.Instance.RemoveXP(upgradeCosts[currentUpgradeLevel]);
            PlayerStats.Instance.IncreaseCardOptions((int)amountToUpgrade);
            currentUpgradeLevel++;
            UpdateCostText();
            UpgradeUIManager.Instance.UpdateStatsText();
        }
        else
        {
            UpgradeUIManager.Instance.ShowNotEnoughXPPanel(upgradeCosts[currentUpgradeLevel]);
            return;
        }
    }
    void UpdateCostText()
    {
        if (currentUpgradeLevel < upgradeCosts.Count)
        {
            upgradeCostText.text = "Upgrade Cost: " + upgradeCosts[currentUpgradeLevel].ToString();
        }
    }

    void Start()
    {
        UpdateCostText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
