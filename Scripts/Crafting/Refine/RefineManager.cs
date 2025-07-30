using Unity.VisualScripting;
using UnityEngine;

public class RefineManager : MonoBehaviour
{
    public static RefineManager Instance;
    public Transform refinePanel;
    public Transform inventoryPanel;
    public GameObject componentRecipePrefab;
    public GameObject inventoryItemPrefab;

    public bool isValid = true;
    private void Awake()
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
    void PopulateRefinePanel()
    {

    }
    void PopulateInventoryPanel()
    {
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in InventorySystem.Instance.itemStacks)
        {
            var itemData = ItemRegistry.Instance.GetItemById(item.itemId);
            if (itemData != null && itemData.itemType == ItemType.Material)
            {
                var itemObject = Instantiate(inventoryItemPrefab, inventoryPanel);
                var itemComponent = itemObject.GetComponent<InventoryItemComponent>();
                if (itemComponent != null)
                {
                    itemComponent.SetItem(itemData, item.quantity, itemData.icon, isValid);
                }
                itemObject.SetActive(true); // Ensure the object is active
            }
        }
    }
    void Start()
    {
        PopulateInventoryPanel();
        PopulateRefinePanel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
