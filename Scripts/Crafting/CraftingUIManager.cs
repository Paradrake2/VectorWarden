using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using System.IO;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class CraftingUIManager : MonoBehaviour
{
    public Transform inventoryParent;
    public Transform recipeSlotsParent; //refining
    public TextMeshProUGUI previewText;
    public Button craftButton;

    public GameObject inventoryButtonPrefab; // these are the inventory buttons the player clicks on that have the items
    public GameObject recipeSlotPrefab;

    public CraftingFactory craftingFactory;
    public CraftingRecipe selectedRecipe;

    private List<Items> currentIngredients = new();
    private List<GameObject> recipeSlotUIs = new();
    public Transform recipeButtonParent; // for recipe prefabs
    public GameObject recipeButtonPrefab;
    public static CraftingUIManager Instance;
    public RefiningUIManager refiningUIManager;
    private Dictionary<string, int> assignedCounts = new(); // key = tag, value = assigned count
    public TextMeshProUGUI recipeInfoText;
    public TextMeshProUGUI itemTagInfo;

    private void Start()
    {
        craftButton.onClick.AddListener(CraftItem);
        PopulateInventory(0);
        GenerateRecipeButtons();
        SetupRecipeSlots();
        Instance = this;
    }
    void CraftItem()
    {
        if (InventorySystem.Instance == null)
        {
            Debug.LogError("InventorySystem.Instance is null!");
            return;
        }
        if (currentIngredients.Count != selectedRecipe.requirements.Sum(req => req.quantityRequired)) return;
        //        Debug.LogWarning(InventorySystem.Instance);
        Equipment result = craftingFactory.GenerateFromIngredients(currentIngredients, selectedRecipe);
        foreach (var ingredients in currentIngredients)
        {
            InventorySystem.Instance.RemoveItem(ingredients.itemName, 1);
        }
        PopulateInventory(selectedRecipe.recipeTier);
        try
        {
            InventorySystem.Instance.AddEquipment(result);
        }
        catch (IOException e)
        {
            Debug.Log(e.StackTrace);
        }
        previewText.text = $"{result.itemName} crafted!";
        currentIngredients.Clear();
    }
    public void DisplayShader(string[] tags, float tier)
    {
        foreach (Transform child in inventoryParent)
        {
            if (tags == null) continue;
            Image overlay = child.Find("Shader")?.GetComponent<Image>();
            if (overlay == null) Debug.LogWarning("no overlay");
            InventoryButtonController btnCtrl = child.GetComponent<InventoryButtonController>();
            if (btnCtrl == null || btnCtrl.item == null || btnCtrl.overlayImage == null) continue;

            bool tagMatch = btnCtrl.item.tags.Any(tag => tags.Any(tags => tag.Equals(tags, StringComparison.OrdinalIgnoreCase)));
            bool tierMatch =  btnCtrl.item.tier >= tier;

            bool isValid = tagMatch && tierMatch;

            if (btnCtrl.overlayImage == null) Debug.LogError("overlay is null");
            btnCtrl.overlayImage.enabled = !isValid;
            overlay.enabled = !isValid;
        }
    }
    public Image GetOverlay()
    {
        Image overlay = transform.Find("Shader")?.GetComponent<Image>();
        return overlay;
    }
    public void PopulateInventory(float tier, string[] validTags = null)
    {
        /*
        // this is for testing
        List<Items> allItems = new List<Items>(Resources.LoadAll<Items>("Items"));

        foreach(Items item in allItems) {
            var button = Instantiate(inventoryButtonPrefab, inventoryParent);
            var iconImage = button.transform.Find("Image").GetComponent<Image>();
            iconImage.sprite = item.icon;
            button.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
            button.GetComponent<Button>().onClick.AddListener(() => {TryAddIngredient(item);});
        }
        */

        // this is for actual use
        //foreach (Transform child in inventoryParent) Destroy(child.gameObject); // clear old buttons
        ClearChildren(inventoryParent);

        foreach (var stack in InventorySystem.Instance.itemStacks)
        {
            Items itemData = ItemRegistry.Instance.GetItemById(stack.itemId);
            if (itemData == null) continue;
            GameObject button = Instantiate(inventoryButtonPrefab, inventoryParent);
            InventoryButtonController btnCtrl = button.GetComponent<InventoryButtonController>();
            btnCtrl.item = itemData;

            // display tags when hover
            var trigger = button.AddComponent<UnityEngine.EventSystems.EventTrigger>();

            // pointer enter
            var enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
            enterEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) => { ShowItemTags(itemData); });
            enterEntry.callback.AddListener((data) => { HoverTextShow(itemData); });
            enterEntry.callback.AddListener((data) => { MaterialStatShow(itemData); });
            trigger.triggers.Add(enterEntry);

            // pointer exit
            var exitEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
            exitEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) => { ClearItemTags(); });
            exitEntry.callback.AddListener((data) => { HideHoverText(); });
            exitEntry.callback.AddListener((data) => { MaterialStatHide(); });
            trigger.triggers.Add(exitEntry);
            Image icon = button.GetComponentInChildren<Image>();
            if (icon != null)
            {
                icon.sprite = itemData.icon;
                icon.preserveAspect = true;
            }

            // Set up text
            TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = $"{stack.quantity}";


            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                TryAddIngredient(itemData);
                refiningUIManager.TryAddIngredient(itemData);
            });
        }
        // Shader overlay

        if (validTags != null) DisplayShader(validTags, tier);
        

    }

    public void MaterialStatShow(Items item)
    {
        if (item.flatDamage != 0) recipeInfoText.text += "Flat Damage: " + item.flatDamage + "\n";
        if (item.damageMult != 0) recipeInfoText.text += "Damage Mult: " + item.damageMult + "\n";
        if (item.flatDefense != 0) recipeInfoText.text += "Flat Defense: " + item.flatDefense + "\n";
        if (item.defenseMult != 0) recipeInfoText.text += "Defense Mult: " + item.defenseMult + "\n";
        if (item.flatMaxHP != 0) recipeInfoText.text += "Flat Max HP: " + item.flatMaxHP + "\n";
        if (item.HPMult != 0) recipeInfoText.text += "HP Mult: " + item.HPMult + "\n";
        if (item.flatPureDamage != 0) recipeInfoText.text += "Flat Pure Damage: " + item.flatPureDamage + "\n";
        if (item.pureDamageMult != 0) recipeInfoText.text += "Pure Damage Mult: " + item.pureDamageMult + "\n";
        //if (item.flatMaxMana != 0) recipeInfoText.text += "Flat Max Mana: " + item.flatMaxMana + "\n";
        //if (item.maxManaMult != 0) recipeInfoText.text += "Max Mana Mult: " + item.maxManaMult + "\n";
        recipeInfoText.text += "Tier: " + item.tier + "\n";
    }
    public void MaterialStatHide()
    {
        recipeInfoText.text = "";
    }
    public void LoadEnhance()
    {
        SceneManager.LoadScene("Enhance");
    }
    void HoverTextShow(Items itemData)
    {
        HoverNameUI.Instance.Show(itemData.itemName);
    }
    void HideHoverText()
    {
        HoverNameUI.Instance.Hide();
    }
    public void ShowItemTags(Items item)
    {
        if (item == null || item.tags == null || item.tags.Length == 0)
        {
            itemTagInfo.text = "Tags: None";
            return;
        }
        string joinedTags = string.Join(", ", item.tags);
        itemTagInfo.text = $"Tags - {joinedTags}";
    }

    public void ClearItemTags()
    {
        itemTagInfo.text = "";
    }

    void SetupRecipeSlots()
    {

        if (selectedRecipe == null) return;
        ClearChildren(recipeSlotsParent);
        //foreach (Transform child in recipeSlotsParent) Destroy(child.gameObject);
        recipeSlotUIs.Clear();
        /*
        for (int i = 0; i < selectedRecipe.requirements.Sum(req => req.quantityRequired); i++) {
            var slot = Instantiate(recipeSlotPrefab, recipeSlotsParent);
            slot.GetComponentInChildren<Text>().text = $"Slot {i + 1}";
            recipeSlotUIs.Add(slot);
        }
        */
        foreach (var req in selectedRecipe.requirements)
        {
            for (int i = 0; i < req.quantityRequired; i++)
            {
                GameObject slot = Instantiate(recipeSlotPrefab, recipeSlotsParent);
                var text = slot.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = $"Needs: {req.requiredTag}";
                recipeSlotUIs.Add(slot);
            }
        }
        if (selectedRecipe != null) DisplayShader(selectedRecipe.GetTags(), selectedRecipe.recipeTier);
    }
    void ClearChildren(Transform parent = null)
    {
        if (parent != null)
        {
            foreach (Transform child in parent) Destroy(child.gameObject);
        }

    }
    void GenerateRecipeButtons()
    {
        Debug.LogWarning("GENERATING RECIPE BUTTONS");
        foreach (var recipe in PlayerRecipeBook.Instance.knownRecipes)
        {
            Debug.Log(recipe.name);
            GameObject buttonObject = Instantiate(recipeButtonPrefab, recipeButtonParent);
            buttonObject.GetComponentInChildren<TextMeshProUGUI>().text = recipe.getRecipeName();
            buttonObject.GetComponentInChildren<Image>().sprite = recipe.getIcon();

            var button = buttonObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => SelectRecipe(recipe));
            }
        }
    }

    public void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;
        currentIngredients.Clear();
        assignedCounts.Clear();
        SetupRecipeSlots();
        //UpdatePreview();
        if (recipe != null)
        {
            recipeInfoText.text = $"<b>{recipe.getRecipeName()}</b>\n";
            foreach (var req in recipe.requirements)
            {
                recipeInfoText.text += $"- {req.requiredTag}: x {req.quantityRequired}\n";
            }
            recipeInfoText.text += $"Tier: {recipe.recipeTier}";
        }
        else
            recipeInfoText.text = "";

    }
    void TryAddIngredient(Items item)
    {
        foreach (var tag in item.tags)
        {
            try
            {
                var req = selectedRecipe.requirements.FirstOrDefault(r => r.requiredTag.Contains(tag)); // was == tag
                if (req != null)
                {
                    if (item.tier < selectedRecipe.recipeTier)
                    {
                        Debug.LogWarning($"{item.itemName} is tier {item.tier}, but recipe requires tier {selectedRecipe.recipeTier} or above");
                        continue;
                    }
                    int alreadyAssigned = assignedCounts.ContainsKey(tag) ? assignedCounts[tag] : 0;
                    if (alreadyAssigned < req.quantityRequired)
                    {
                        int countAlreadyUsed = currentIngredients.Count(i => i.itemName == item.itemName);
                        int available = InventorySystem.Instance.GetQuantity(item.itemName);
                        if (countAlreadyUsed >= available)
                        {
                            Debug.Log($"Cannot add more {item.itemName}!");
                            continue;
                        }
                        Debug.Log(alreadyAssigned + "<- number ingredient assigned");
                        currentIngredients.Add(item);
                        if (!assignedCounts.ContainsKey(tag)) assignedCounts[tag] = 0;
                        assignedCounts[tag]++;

                        int slotIndex = currentIngredients.Count - 1;
                        if (slotIndex < recipeSlotUIs.Count)
                        {
                            var text = recipeSlotUIs[slotIndex].GetComponentInChildren<TextMeshProUGUI>();
                            if (text != null) text.text = item.itemName;

                            var image = recipeSlotUIs[slotIndex].GetComponentInChildren<Image>();
                            if (image != null) image.sprite = item.icon;
                        }

                        UpdatePreview();
                        return;
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Debug.Log(e.StackTrace);
            }
        }

    }
    public void OpenCoreRefine(GameObject menu)
    {
        menu.SetActive(true);
        PowderMaker.Instance.PopulateCoreInventory();
    }
    void UpdatePreview()
    {
        if (currentIngredients.Count != selectedRecipe.requirements.Sum(req => req.quantityRequired))
        {
            previewText.text = "Select all ingredients...";
            return;
        }

        Equipment preview = craftingFactory.PreviewCraftedEquipment(currentIngredients, selectedRecipe);
        string result = $"<b>{preview.itemName}</b>\n\n";

        foreach (var mod in preview.modifiers)
        {
            result += $"{mod.statType}: + {mod.flatAmount}";
            if (mod.percentAmount != 0) result += $" (+{mod.percentAmount * 100}% bonus)";
            result += "\n";
        }

        previewText.text = result;
    }
    public void LoadMagicCreator()
    {
        SceneManager.LoadScene("MagicCreator");
    }
}
