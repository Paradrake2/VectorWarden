using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public UpgradeUIManager upgradeUIManager;
    public PlayerStats playerStats;
    public float UpgradeXPAmount;
    public TextMeshProUGUI xpText;
    void Start()
    {
        Instance = this;
        playerStats = PlayerStats.Instance;
        xpText.text = UpgradeXPAmount.ToString();
    }

    public float GetUpgradeXPAmount()
    {
        return UpgradeXPAmount;
    }
    public void RemoveXP(float amount)
    {
        UpgradeXPAmount -= amount;
    }
    public void UpdateXPText()
    {
        xpText.text = UpgradeXPAmount.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
