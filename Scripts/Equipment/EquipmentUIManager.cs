using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUIManager : MonoBehaviour
{
    public Transform inventoryPanel;
    public static EquipmentUIManager Instance;
    public GameObject equipmentButtonPrefab;
    public List<EquipmentSlotManager> slotManagers;
    public EquipmentInventoryManager equipmentInventoryManager;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        int accessoryCounter = 0;
        foreach (var slot in slotManagers)
        {
            if (slot.isAccessory)
            {
                slot.accessoryIndex = accessoryCounter;
                accessoryCounter++;
            }
        }

        RefreshSlots();
        equipmentInventoryManager.PopulateInventory((Equipment item) =>
        {
            PlayerStats.Instance.EquipItem(item);
            RefreshSlots();
            RefreshInventoryUI();
        });
    }
    public void RefreshInventoryUI()
    {
        equipmentInventoryManager.PopulateInventory((Equipment item) =>
        {
            PlayerStats.Instance.EquipItem(item);
            RefreshSlots();
            RefreshInventoryUI();
        });
    }
    public void RefreshSlots()
    {
        foreach (var slot in slotManagers)
        {
            if (slot.isAccessory)
            {
                Equipment accessory = PlayerStats.Instance.GetAccessoryAt(slot.accessoryIndex);
                Image slotImage = slot.GetComponentInChildren<Image>();
                slot.SetItem(accessory);
                if (accessory != null)
                {
                    slotImage.sprite = accessory.icon;
                }
                else
                {
                    slotImage.sprite = null;
                }
            }
            else
            {
                if (PlayerStats.Instance.equippedItems.TryGetValue(slot.slotType, out var equippedItem))
                {
                    slot.SetItem(equippedItem);
                    Image slotImage = slot.GetComponentInChildren<Image>();
                    if (equippedItem != null)
                    {
                        slotImage.sprite = equippedItem.icon;
                    }
                    else
                    {
                        slotImage.sprite = null;
                    }
                }
                else
                {
                    slot.SetItem(null);
                    Image slotImage = slot.GetComponentInChildren<Image>();
                    slotImage.sprite = null;
                }
            }
        }
    }
    void EquipToSlot(Equipment item) {
        PlayerStats.Instance.EquipItem(item);
        RefreshSlots();
        // Update visuals
    }
}
