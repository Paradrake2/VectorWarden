using UnityEngine;
using UnityEngine.UI;

public class UpgradeMaterialInventoryManager : MonoBehaviour
{
    public static UpgradeMaterialInventoryManager Instance;
    public GameObject inventoryPanel;
    public Transform inventoryContent;
    public GameObject inventoryItemPrefab;

    void Start()
    {
        Instance = this;
        PopulateInventory();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PopulateInventory()
    {
        if (inventoryContent == null)
        {
            Debug.LogError("Inventory content is not assigned.");
            return;
        }

        // Clear existing items
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }

        // Populate with items from the player's inventory
        foreach (var item in InventorySystem.Instance.itemStacks)
        {
            var itemData = ItemRegistry.Instance.GetItemById(item.itemId);
            if (itemData != null && itemData.itemType == ItemType.Material)
            {
                var itemObject = Instantiate(inventoryItemPrefab, inventoryContent);
                itemObject.SetActive(true); // Ensure the object is active
                var itemComponent = itemObject.GetComponent<InventoryItemComponent>();
                if (itemComponent != null)
                {
                    itemComponent.SetItem(itemData, item.quantity, itemData.icon, false);
                }

                itemObject.GetComponent<Image>().enabled = true;
                itemObject.GetComponent<Button>().enabled = true;
                itemObject.GetComponent<InventoryItemComponent>().enabled = true;
            }
        }
    }
    // Hover text pops up the name and quantity when hovering over item slot
}
