using UnityEngine;

public class UpgradeXPHolder : MonoBehaviour
{
    public static UpgradeXPHolder Instance;
    public float UpgradeXPAmount;
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
    public float GetUpgradeXPAmount()
    {
        return UpgradeXPAmount;
    }
    public void SetUpgradeXPAmount(float amount)
    {
        UpgradeXPAmount = amount;
    }
}
