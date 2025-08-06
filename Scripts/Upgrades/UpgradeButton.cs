using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    public PlayerStats playerStats;
    
    
    void Start()
    {
        playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats instance not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
