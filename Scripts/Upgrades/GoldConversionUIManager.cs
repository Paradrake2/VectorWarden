using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoldConversionUIManager : MonoBehaviour
{
    public TextMeshProUGUI goldAmountText;
    public TMP_InputField xpInputField;
    public Button convertButton;
    public PlayerStats playerStats;
    public GameObject itemConvertButton;
    public Transform itemConvertButtonParent;
    public static GoldConversionUIManager Instance;

    public float GoldXPConversionRate = 0.25f;
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        SetupButtons();
        PopulateInventory();
        UpdateGoldText();
    }

    // --- Core actions ---
    public void ConvertGoldToXP()
    {
        float rawDesiredXP = ParseXPInputRaw();
        float xpToBuy = ClampXPAffordable(rawDesiredXP);
        if (xpToBuy <= 0f) return;

        float goldCost = Mathf.CeilToInt(xpToBuy / GoldXPConversionRate);
        goldCost = Mathf.Min(goldCost, playerStats.GetGoldAmount()); // safety clamp

        playerStats.RemoveGold(goldCost);
        UpgradeManager.Instance.AddXP(xpToBuy);

        // Clear the field and refresh UI
        xpInputField.SetTextWithoutNotify("");
        UpdateGoldText();
        UpdateButtonState();
        UpgradeManager.Instance.UpdateXPText();
    }

    // --- UI wiring ---
    void SetupButtons()
    {
        convertButton.onClick.AddListener(ConvertGoldToXP);
        xpInputField.onValueChanged.AddListener(OnInputChanged);
        UpdateButtonState();
    }

    void OnInputChanged(string _)
    {
        float raw = ParseXPInputRaw();
        float clamped = ClampXPAffordable(raw);

        // If user typed more than affordable, rewrite the field to the max
        if (!Mathf.Approximately(raw, clamped))
            xpInputField.SetTextWithoutNotify(clamped.ToString("0"));

        UpdateButtonState();
    }

    void UpdateButtonState()
    {
        convertButton.interactable = ParseXPInputRaw() > 0f && MaxXPAffordable() > 0f;
    }

    void UpdateGoldText()
    {
        goldAmountText.text = playerStats.GetGoldAmount().ToString();
    }

    // --- Helpers ---
    float MaxXPAffordable()
    {
        return playerStats.GetGoldAmount() * GoldXPConversionRate;
    }

    float ClampXPAffordable(float xp)
    {
        return Mathf.Clamp(xp, 0f, MaxXPAffordable());
    }

    float ParseXPInputRaw()
    {
        if (string.IsNullOrWhiteSpace(xpInputField.text)) return 0f;
        if (float.TryParse(xpInputField.text, out float xpAmount))
            return Mathf.Max(0f, xpAmount);
        return 0f;
    }

    public void PopulateInventory()
    {
        foreach (Transform child in itemConvertButtonParent)
            Destroy(child.gameObject);

        foreach (var item in InventorySystem.Instance.acquiredItemsList)
        {
            GameObject button = Instantiate(itemConvertButton, itemConvertButtonParent);
            var gc = button.GetComponent<GoldConversionButton>();
            gc.convertedItem = item;
            gc.goldNeeded = item.goldCost;
            gc.itemIcon.sprite = item.icon;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
