using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GoldConversionButton : MonoBehaviour
{
    public Items convertedItem;
    public PlayerStats playerStats;
    public float goldNeeded;
    public Image itemIcon;
    public TextMeshProUGUI goldNeededText;
    public void ConvertGoldToItem()
    {
        if (playerStats.HasEnoughGold(goldNeeded))
        {
            playerStats.RemoveGold(goldNeeded);
            InventorySystem.Instance.AddItem(convertedItem.ID, 1);
            // Populate inventory
            UpgradeMaterialInventoryManager.Instance.PopulateInventory();
        }
        else
        {
            // pop up panel saying not enough gold
        }
    }
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        if (itemIcon == null)
        {
            itemIcon.sprite = convertedItem.icon;
        }
        SetGoldCost();
    }

    void SetGoldCost()
    {
        goldNeededText.text = $"{goldNeeded}";
    }

}
