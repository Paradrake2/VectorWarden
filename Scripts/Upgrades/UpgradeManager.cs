using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public UpgradeUIManager upgradeUIManager;
    public PlayerStats playerStats;
    public float UpgradeXPAmount;
    void Start()
    {
        Instance = this;
        playerStats = PlayerStats.Instance;
    }

    public float GetUpgradeXPAmount()
    {
        return UpgradeXPAmount;
    }
    public void RemoveXP(float amount)
    {
        UpgradeXPAmount -= amount;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
