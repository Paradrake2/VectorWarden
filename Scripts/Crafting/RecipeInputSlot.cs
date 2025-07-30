using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RecipeInputSlot
{
    //public string requiredTag;
    public List<string> requiredTags = new();
    public int quantityRequired;
    public Items assignedItem;

    public void SetSlot(string tag, int qty)
    {
        requiredTags = new List<string> { tag };
        quantityRequired = qty;
    }

    public bool IsValidItem(Items item)
    {
        return item.tags.Any(tag => requiredTags.Contains(tag));
    }
}
