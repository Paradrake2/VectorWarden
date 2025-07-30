using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EquipmentButton : MonoBehaviour
{
    public bool isAccessory = false;
    public EquipmentSlot slotType;
    public int accessoryIndex;
    private Button button;
    public EquipmentInventoryManager equipmentInventoryManager;
    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    private void EquipmentMenuTasks() {
        Debug.Log("Called EquipmentMenuTasks");
        if (!isAccessory)
        {
            Equipment equippedItem = PlayerStats.Instance.GetEquippedItem(slotType);

            if (equippedItem != null)
            {
                Debug.Log($"Clicked {slotType} slot: Equipped {equippedItem.itemName}");
                // TODO: open equipment details/unequip/replace
                PlayerStats.Instance.UnequipItem(equippedItem);
                EquipmentUIManager.Instance.RefreshSlots();

                equipmentInventoryManager.PopulateInventory((Equipment item) =>
                {
                    PlayerStats.Instance.EquipItem(item);
                    EquipmentUIManager.Instance.RefreshSlots();
                    EquipmentUIManager.Instance.RefreshInventoryUI();
                });
            }
            else
            {
                Debug.Log($"Clicked {slotType} slot: Empty");
            }
        }
        else
        {
            Equipment accessoryItem = PlayerStats.Instance.GetAccessoryAt(accessoryIndex);
            if (accessoryItem != null)
            {
                Debug.Log($"Clicked Accessory Slot {accessoryIndex}: Equipped {accessoryItem.itemName}");
                PlayerStats.Instance.UnequipItem(accessoryItem);
                EquipmentUIManager.Instance.RefreshSlots();

                equipmentInventoryManager.PopulateInventory((Equipment item) =>
                {
                    Debug.Log(item.ID + " <- selected item");
                    PlayerStats.Instance.EquipItem(item);
                    EquipmentUIManager.Instance.RefreshSlots();
                    EquipmentUIManager.Instance.RefreshInventoryUI();
                });
                
            }
            else
            {
                Debug.Log($"Clicked Accessory Slot {accessoryIndex}: Empty");
            }
        }
    }
    private void OnClick() {
        Debug.Log("Clicked");
        EquipmentMenuTasks();
        if (SceneManager.GetActiveScene().ToString() == "EquipmentMenu")
        {
            EquipmentMenuTasks();
        }
        
    }
}
