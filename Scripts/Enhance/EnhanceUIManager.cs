using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnhanceUIManager : MonoBehaviour
{
    public static EnhanceUIManager Instance;
    public Transform inventoryParent;
    public GameObject enhanceInventoryButton;
    public Transform enhanceItemParent;
    public GameObject enhanceItemSlot;
    public TextMeshProUGUI succcessChance;
    public TextMeshProUGUI hoverText;
    public EnhanceItem enhanceItem;
    public Items selectedItem;
    public TextMeshProUGUI itemInfoText;
    public GameObject notEnoughPowder;
    public GameObject failedEnhance;
    public GameObject enhanceLimitReachedGameObject;
    public TextMeshProUGUI powderCost;
    private Vector2 offset = new Vector2(15, -15);
    public void PopulateEnhanceInventory()
    {
        ClearChildren(inventoryParent);

        foreach (var stack in InventorySystem.Instance.itemStacks)
        {
            Items itemData = ItemRegistry.Instance.GetItemById(stack.itemId);
            if (itemData == null) continue;
            GameObject button = Instantiate(enhanceInventoryButton, inventoryParent);
            SetupEnhanceInventoryButtons(button, itemData, stack.quantity);

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                SelectEnhanceItem(itemData);
            });
        }
    }


    void SetupEnhanceInventoryButtons(GameObject button, Items item, int quantity)
    {
        Image icon = button.GetComponentInChildren<Image>();
        icon.sprite = item.icon;
        icon.preserveAspect = true;
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "x" + quantity;
        var trigger = button.AddComponent<UnityEngine.EventSystems.EventTrigger>();

        // Pointer enter
        var enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        enterEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => { HoverTextShow(item); });
        trigger.triggers.Add(enterEntry);

        var exitEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        exitEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => { HideHoverText(); });
        trigger.triggers.Add(exitEntry);
    }
    public void SelectEnhanceItem(Items item)
    {
        selectedItem = item;
        DisplaySelectedItem(item);
        UpdateSuccessChance(selectedItem);
    }
    void HoverTextShow(Items item)
    {
        hoverText.text = item.itemName;
        hoverText.gameObject.SetActive(true);
    }
    void HideHoverText()
    {
        hoverText.text = "";
        hoverText.gameObject.SetActive(false);
    }
    

    public void UpdateSuccessChance(Items item)
    {
        succcessChance.text = (enhanceItem.GetSuccessChance(selectedItem) * 100).ToString() + "%";
    }


    bool HasEnoughItems()
    {
        int quantity = InventorySystem.Instance.GetQuantity(selectedItem.itemName);
        if (quantity > 0) return true;
        return false;
    }


    void DisplaySelectedItem(Items item)
    {
        Image icon = IconSlot();
        icon.sprite = item.icon;
        DisplayStats();
        DisplayPowderCost();
    }
    void DisplayStats()
    {
        itemInfoText.text = ""; // clear it
        itemInfoText.text += "Item stats: " + "\n";
        if (selectedItem != null)
        {
            // Display powder cost
            if (selectedItem.flatDamage != 0) itemInfoText.text += "Flat Damage: " + selectedItem.flatDamage + " + 1" + "\n";
            if (selectedItem.damageMult != 0) itemInfoText.text += "Damage Mult: " + selectedItem.damageMult + " + 0.01" + "\n";
            if (selectedItem.flatDefense != 0) itemInfoText.text += "Flat Defense: " + selectedItem.flatDefense + " + 1" + "\n";
            if (selectedItem.defenseMult != 0) itemInfoText.text += "Defense Mult: " + selectedItem.defenseMult + " + 0.01" + "\n";
            if (selectedItem.flatMaxHP != 0) itemInfoText.text += "Flat Max HP: " + selectedItem.flatMaxHP + " + 10" + "\n";
            if (selectedItem.HPMult != 0) itemInfoText.text += "HP Mult: " + selectedItem.HPMult + " + 0.01" + "\n";
            if (selectedItem.flatPureDamage != 0) itemInfoText.text += "Flat Pure Damage: " + selectedItem.flatPureDamage + "\n";
            if (selectedItem.pureDamageMult != 0) itemInfoText.text += "Pure Damage Mult: " + selectedItem.pureDamageMult + "\n";
            if (selectedItem.flatMaxMana != 0) itemInfoText.text += "Flat Max Mana: " + selectedItem.flatMaxMana + "\n";
            if (selectedItem.maxManaMult != 0) itemInfoText.text += "Max Mana Mult: " + selectedItem.maxManaMult + "\n";
        }
    }


    public void EnhanceItem()
    {
        if (HasEnoughItems())
        {
            enhanceItem.EnhanceMaterial(selectedItem);
            PopulateEnhanceInventory();
            CheckIfEnough();
        }
        DisplayStats();
    }


    void CheckIfEnough()
    {
        int quantity = InventorySystem.Instance.GetQuantity(selectedItem.itemName);
        if (quantity == 0)
        {
            SetIconToNull();
        }
    }


    void SetIconToNull()
    {
        selectedItem = null;
        Image icon = IconSlot();
        icon.sprite = null;
    }
    Image IconSlot()
    {
        return enhanceItemSlot.GetComponentInChildren<Image>();
    }


    void ClearChildren(Transform parent = null)
    {
        foreach (Transform child in parent) Destroy(child.gameObject);
    }


    public void DisplayPowderCost()
    {
        powderCost.text = "Powder Cost: " + enhanceItem.GetPowderCost(selectedItem).ToString();
    }
    public void NotEnoughPowder()
    {
        StartCoroutine(DisplayNotEnoughPowder());
    }
    private IEnumerator DisplayNotEnoughPowder()
    {
        notEnoughPowder.SetActive(true);
        yield return new WaitForSeconds(1f);
        notEnoughPowder.SetActive(false);
    }


    public void FailedEnhance()
    {
        StartCoroutine(DisplayFailedEnhance());
    }
    private IEnumerator DisplayFailedEnhance()
    {
        failedEnhance.SetActive(true);
        yield return new WaitForSeconds(1f);
        failedEnhance.SetActive(false);
    }


    public void EnhanceLimitReached()
    {
        StartCoroutine(EnhanceLimitReachedRoutine());
    }

    private IEnumerator EnhanceLimitReachedRoutine()
    {
        enhanceLimitReachedGameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        enhanceLimitReachedGameObject.SetActive(false);
    }
    public void LoadCrafting()
    {
        SceneManager.LoadScene("Menu");
    }


    void Start()
    {
        PopulateEnhanceInventory();
        notEnoughPowder.SetActive(false);
        failedEnhance.SetActive(false);
    }

    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        hoverText.rectTransform.position = mousePos + offset;
    }
}
