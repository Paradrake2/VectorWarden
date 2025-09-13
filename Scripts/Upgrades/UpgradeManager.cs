using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public UpgradeXPHolder uxh;
    public UpgradeUIManager upgradeUIManager;
    public PlayerStats playerStats;
    public float UpgradeXPAmount;
    public TextMeshProUGUI xpText;
    void Start()
    {
        uxh = UpgradeXPHolder.Instance;
        UpgradeXPAmount = uxh.GetUpgradeXPAmount();
        Instance = this;
        playerStats = PlayerStats.Instance;
        ConvertToXP();
        xpText.text = UpgradeXPAmount.ToString();
    }
    void ConvertToXP()
    {
        var itemsToRemove = new List<(string itemId, int quantity)>();
        foreach (var item in InventorySystem.Instance.itemStacks)
        {
            var itemData = ItemRegistry.Instance.GetItemById(item.itemId);
            if (itemData != null && itemData.itemType == ItemType.XPItem)
            {
                UpgradeXPAmount += itemData.xpValue * item.quantity;
                itemsToRemove.Add((item.itemId, item.quantity));
            }
        }
        foreach (var (itemId, quantity) in itemsToRemove)
        {
            InventorySystem.Instance.RemoveItem(itemId, quantity);
        }
    }
    public float GetUpgradeXPAmount()
    {
        return UpgradeXPAmount;
    }
    public void RemoveXP(float amount)
    {
        UpgradeXPAmount -= amount;
    }
    public void AddXP(float amount)
    {
        UpgradeXPAmount += amount;
        uxh.SetUpgradeXPAmount(UpgradeXPAmount);
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
