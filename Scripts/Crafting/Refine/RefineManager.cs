using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RefineManager : MonoBehaviour
{
    public static RefineManager Instance;
    public Transform refinePanel;
    public Transform inventoryPanel;
    public GameObject componentRecipePrefab;
    public GameObject inventoryItemPrefab;

    private HashSet<string> validRecipes = new HashSet<string>
    {
        "Recipe1",
        "Recipe2",
        "Recipe3"
    };

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
        foreach (Transform child in refinePanel)
        {
            Destroy(child.gameObject);
        }

        RefineRecipe[] allRecipes = Resources.LoadAll<RefineRecipe>("RefineRecipe");
        var playerMaterialTags = new HashSet<string>();
        foreach (var item in InventorySystem.Instance.itemStacks)
        {
            var itemData = ItemRegistry.Instance.GetItemById(item.itemId);
            if (itemData != null && itemData.itemType == ItemType.Material)
            {
                foreach (var tag in itemData.tags)
                {
                    playerMaterialTags.Add(tag);
                }
            }
        }
        List<RefineRecipe> validRecipesList = new List<RefineRecipe>();
        foreach (var recipe in allRecipes)
        {
            if (playerMaterialTags.Overlaps(recipe.validMaterialTags))
            {
                validRecipesList.Add(recipe);
            }
        }
        // Display all recipes
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
                itemObject.SetActive(true); // Ensure the object is active
                var itemComponent = itemObject.GetComponent<InventoryItemComponent>();
                if (itemComponent != null)
                {
                    itemComponent.SetItem(itemData, item.quantity, itemData.icon, isValid);
                }

                itemObject.GetComponent<Image>().enabled = true;
                itemObject.GetComponent<Button>().enabled = true;
                itemObject.GetComponent<InventoryItemComponent>().enabled = true;
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
