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
        shaderImage.gameObject.SetActive(shaderActive);
        item = itemData;
        quantity = itemQuantity;
        itemIcon = icon;

        // Print states before setting

        if (iconImage != null)
        {
            iconImage.sprite = itemIcon;
            iconImage.enabled = true;
        }
        if (quantityText != null)
        {
            quantityText.text = quantity.ToString();
            quantityText.enabled = quantity > 1; // Only show if quantity is more than 1
        }


    }
}
