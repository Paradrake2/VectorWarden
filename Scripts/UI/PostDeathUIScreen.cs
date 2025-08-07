using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PostDeathUIScreen : MonoBehaviour
{
    public static PostDeathUIScreen Instance;
    public GameObject postDeathPanel;
    public Transform spoilsInventoryPanel;
    public GameObject spoilsInventoryButtonPrefab;
    public TextMeshProUGUI deathMessageText;

    void Start()
    {
        Instance = this;
        postDeathPanel.SetActive(false);
    }
    public void ShowPostDeathScreen(string name)
    {
        postDeathPanel.SetActive(true);
        deathMessageText.text = $"You have died to {name}. Please choose an option to continue.";
        PopulateInventory();
    }
    void PopulateInventory()
    {
        if (spoilsInventoryPanel == null)
        {
            Debug.LogError("Inventory content is not assigned");
            return;
        }

        // Clear existing items
        foreach (Transform child in spoilsInventoryPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in InventorySystem.Instance.acquiredItems)
        {
            var itemData = ItemRegistry.Instance.GetItemById(item.itemId);
            var itemObject = Instantiate(spoilsInventoryButtonPrefab, spoilsInventoryPanel);
            itemObject.SetActive(true);

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
    public void ToMenu()
    {
        AddSpoilsToItems();
        SceneManager.LoadScene("Upgrade");
        Time.timeScale = 1;
    }
    void AddSpoilsToItems()
    {
        foreach (var item in InventorySystem.Instance.acquiredItems)
        {
            InventorySystem.Instance.AddItem(item.itemId, item.quantity);
        }
        InventorySystem.Instance.acquiredItems.Clear();
    }
}
