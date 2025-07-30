using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class InventoryItemComponent : MonoBehaviour
{
    public Items item;
    public int quantity;
    public Sprite itemIcon;

    public Image iconImage;
    public TextMeshProUGUI quantityText;
    public Image shaderImage;
    public void SetItem(Items itemData, int itemQuantity, Sprite icon, bool shaderActive)
    {
        //shaderImage.gameObject.SetActive(shaderActive);
        item = itemData;
        quantity = itemQuantity;
        itemIcon = icon;

        // Print states before setting
        Debug.Log($"iconImage.enabled (before): {iconImage.enabled}");
        Debug.Log($"iconImage.gameObject.activeSelf (before): {iconImage.gameObject.activeSelf}");
        Debug.Log($"quantityText.enabled (before): {quantityText.enabled}");
        Debug.Log($"quantityText.gameObject.activeSelf (before): {quantityText.gameObject.activeSelf}");

        if (iconImage != null)
        {
            iconImage.sprite = itemIcon;
            iconImage.enabled = true;
        }
        if (quantityText != null)
        {
            quantityText.text = quantity.ToString();
        }

        // Print states after setting
        Debug.Log($"iconImage.enabled (after): {iconImage.enabled}");
        Debug.Log($"iconImage.gameObject.activeSelf (after): {iconImage.gameObject.activeSelf}");
        Debug.Log($"quantityText.enabled (after): {quantityText.enabled}");
        Debug.Log($"quantityText.gameObject.activeSelf (after): {quantityText.gameObject.activeSelf}");
        Debug.Log("Instantiated object active: " + gameObject.activeSelf);
        if (!quantityText.enabled)
        {
            Debug.LogWarning("quantityText was disabled! " + new System.Diagnostics.StackTrace());
        }
    }

    void Awake() {
        Debug.Log($"{gameObject.name} Awake: iconImage.enabled={iconImage.enabled}, quantityText.enabled={quantityText.enabled}");
    }
}
