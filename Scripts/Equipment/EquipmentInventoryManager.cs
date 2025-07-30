using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class EquipmentInventoryManager : MonoBehaviour
{
    public static EquipmentInventoryManager Instance;
    public Transform inventoryPanel;
    public GameObject equipmentButtonPrefab;

    public void PopulateInventory(Action<Equipment> onClickAction)
    {
        foreach (Transform child in inventoryPanel)
            Destroy(child.gameObject); // Clear existing buttons

        foreach (var item in PlayerStats.Instance.ownedGear)
        {
            GameObject btn = Instantiate(equipmentButtonPrefab, inventoryPanel);
            var icon = btn.GetComponentInChildren<Image>();
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();

            if (icon != null) icon.sprite = item.icon;
            if (text != null) text.text = item.itemName;

            Equipment capturedItem = item;
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log($"Button clicked: {capturedItem.ID}");
                onClickAction?.Invoke(capturedItem);
            });
        }
    }

    
}
