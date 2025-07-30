using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

public class RefiningUIManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Transform recipeListParent;
    public GameObject recipeButtonPrefab;
    public GameObject refinePowderPrefab;
    public TextMeshProUGUI ingredientText;
    public TextMeshProUGUI statstext;
    public Button refineButton;
    public Image itemIcon;
    public static RefiningUIManager Instance;

    [Header("New refine stuff")]
    public RefineRecipe selectedRecipe;
    public List<Items> currentIngredients = new();
    private List<GameObject> recipeSlotUIs = new();
    private Dictionary<string, int> assignedCounts = new();

    public Transform recipeButtonParent;
    public TextMeshProUGUI previewText;
    public CraftingUIManager craftingUIManager;
    public InventorySystem inventorySystem;

    public List<Items> lastUsedIngredients = new();
    private bool isHolding = false;
    public float holdInterval = 0.2f;

    public TMP_InputField searchInput;
    private Coroutine debounceCoroutine;

    void Start()
    {
        refineButton.onClick.AddListener(RefineItem);
        inventorySystem = FindFirstObjectByType<InventorySystem>();
        searchInput.onValueChanged.AddListener((text) => {
            if (debounceCoroutine != null) StopCoroutine(debounceCoroutine);
            debounceCoroutine = StartCoroutine(DelayedFilter(text));
        });
        GenerateRecipeButtons();
    }

    private IEnumerator DelayedFilter(string text)
    {
        yield return null;
        GenerateRecipeButtons();
    }

    void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        StartCoroutine(RefineLoop());
    }

    private IEnumerator RefineLoop()
    {
        while (isHolding)
        {
            refineButton.onClick.Invoke();
            yield return new WaitForSeconds(holdInterval);
        }
    }

    void RefineItem()
    {
        if (currentIngredients.Count != selectedRecipe.requirements.Sum(req => req.quantityRequired)) return;
        bool canRefineAgain = true;
        foreach (var item in currentIngredients)
        {
            if (!InventorySystem.Instance.HasItem(item.ID, 1))
            {
                canRefineAgain = false;
                break;
            }
        }

        foreach (var item in currentIngredients)
        {
            InventorySystem.Instance.RemoveItem(item.ID, 1);
        }
        lastUsedIngredients = new List<Items>(currentIngredients);
        inventorySystem.AddItem(selectedRecipe.outputItem.ID, selectedRecipe.outputQuantity);
        inventorySystem.discoveredRefinedItems.Add(selectedRecipe.outputItem.ID);
        StartCoroutine(CreatedItemRoutine());
        currentIngredients.Clear();
        assignedCounts = new Dictionary<string, int>();
        craftingUIManager.PopulateInventory(0);
        SetupRecipeSlots();
        if (canRefineAgain)
        {
            TryAutoAssignLastUsedIngredients();
        }
    }

    private IEnumerator CreatedItemRoutine()
    {
        previewText.text = $"{selectedRecipe.outputItem.itemName} created!";
        previewText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        previewText.gameObject.SetActive(false);
    }

    void TryAutoAssignLastUsedIngredients()
    {
        foreach (var item in lastUsedIngredients)
        {
            if (inventorySystem.HasItem(item.ID, 1))
            {
                TryAddIngredient(item);
            }
        }
    }

    public void GenerateRecipeButtons()
    {
        HoverNameUI.Instance.Hide();
        RefineRecipe[] allRecipes = Resources.LoadAll<RefineRecipe>("RefineRecipe");
        foreach (Transform child in recipeListParent) Destroy(child.gameObject);

        string query = searchInput.text.ToLower();

        foreach (var recipe in allRecipes)
        {
            bool matchesSearch = string.IsNullOrEmpty(query) || (!string.IsNullOrEmpty(recipe.recipeName) && recipe.recipeName.ToLower().Contains(query)) ||
            (recipe.GetTags() != null && recipe.GetTags().Any(tag => !string.IsNullOrEmpty(tag) && tag.ToLower().Contains(query)));

            if (!matchesSearch) continue;

            bool canCraftNow = true;
#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                foreach (var req in recipe.requirements)
                {
                    if (!inventorySystem.HasItemsWithTag(req.requiredTag, req.quantityRequired))
                    {
                        canCraftNow = false;
                        break;
                    }
                }

                bool hasCraftedBefore = inventorySystem.discoveredRefinedItems.Contains(recipe.outputItem.ID);
                if (canCraftNow || hasCraftedBefore)
                {
                    GameObject btn = Instantiate(recipeButtonPrefab, recipeListParent);
                    var eventTrigger = btn.AddComponent<EventTrigger>();

                    btn.GetComponentInChildren<Image>().sprite = recipe.icon;

                    var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                    enter.callback.AddListener((eventData) => { HoverNameUI.Instance.Show(recipe.outputItem.itemName); });
                    enter.callback.AddListener((eventData) => { craftingUIManager.MaterialStatShow(recipe.outputItem); });
                    eventTrigger.triggers.Add(enter);

                    var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                    exit.callback.AddListener((eventData) => { HoverNameUI.Instance.Hide(); });
                    exit.callback.AddListener((eventData) => { craftingUIManager.MaterialStatHide(); });
                    eventTrigger.triggers.Add(exit);

                    btn.GetComponent<Button>().onClick.AddListener(() => { SelectRecipe(recipe); });
                }
            }
            catch (NullReferenceException e)
            {
                // Safely ignore malformed recipes
            }
#pragma warning restore CS0168 // Variable is declared but never used
        }

        craftingUIManager.PopulateInventory(0);
        craftingUIManager.recipeInfoText.text = "";
    }

    void SelectRecipe(RefineRecipe recipe)
    {
        selectedRecipe = recipe;
        currentIngredients.Clear();
        assignedCounts = new Dictionary<string, int>();
        SetupRecipeSlots();

        CraftingUIManager.Instance.recipeInfoText.text = $"<b>{recipe.recipeName}</b>\n";
        foreach (var req in recipe.requirements)
        {
            CraftingUIManager.Instance.recipeInfoText.text += $"- {req.requiredTag}: x{req.quantityRequired}\n";
        }
    }

    void SetupRecipeSlots()
    {
        HoverNameUI.Instance.Hide();
        foreach (Transform child in recipeButtonParent) Destroy(child.gameObject);

        recipeSlotUIs.Clear();
        if (selectedRecipe == null) return;

        foreach (var req in selectedRecipe.requirements)
        {
            for (int i = 0; i < req.quantityRequired; i++)
            {
                GameObject slot = Instantiate(recipeButtonPrefab, recipeButtonParent);
                var text = slot.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = $"Needs: {req.requiredTag}";
                recipeSlotUIs.Add(slot);
            }
        }

        if (selectedRecipe != null)
        {
            craftingUIManager.DisplayShader(selectedRecipe.GetTags(), 0);
            craftingUIManager.PopulateInventory(0, selectedRecipe.GetTags());
        }
    }

    public void TryAddIngredient(Items item)
    {
        foreach (var tag in item.tags)
        {
            var req = selectedRecipe.requirements.FirstOrDefault(r => r.requiredTag == tag);
            if (req != null)
            {
                if (item.tier < selectedRecipe.tier)
                {
                    // pop up warning that item isnt high enough tier
                    continue;
                }
                int alreadyAssigned = assignedCounts.ContainsKey(tag) ? assignedCounts[tag] : 0;
                if (alreadyAssigned < req.quantityRequired)
                {
                    int alreadyUsed = currentIngredients.Count(i => i.itemName == item.itemName);
                    int available = InventorySystem.Instance.GetQuantity(item.itemName);
                    if (alreadyUsed >= available) continue;

                    currentIngredients.Add(item);
                    if (!assignedCounts.ContainsKey(tag)) assignedCounts[tag] = 0;
                    assignedCounts[tag]++;

                    int slotIndex = currentIngredients.Count - 1;
                    if (slotIndex < recipeSlotUIs.Count)
                    {
                        var text = recipeSlotUIs[slotIndex].GetComponentInChildren<TextMeshProUGUI>();
                        var icon = recipeSlotUIs[slotIndex].GetComponentInChildren<Image>();
                        if (text != null) text.text = item.itemName;
                        if (icon != null) icon.sprite = item.icon;
                    }

                    return;
                }
            }
        }
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        OnPointerDown(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
    }
}
