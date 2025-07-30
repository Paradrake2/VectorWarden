using UnityEngine;

public class RefiningManager : MonoBehaviour
{
    public static RefiningManager Instance;
    public RefiningUIManager refiningUIManager;
    void Awake()
    {
        Instance = this;
        Debug.LogWarning("Refining manager initialized");
    }
    /*
    public bool CanRefine(Items refinedItem) {
        for (int i = 0; i < refinedItem.requiredMaterials.Count; i++) {
            string id = refinedItem.requiredMaterials[i].ID;
            int required = refinedItem.requiredQuantities[i];
            if (!InventorySystem.Instance.HasItem(id, required)) return false;
        }
        return true;
    }
    public void Refine(Items refinedItem) {
        if (!CanRefine(refinedItem)) return;

        for (int i = 0; i < refinedItem.requiredMaterials.Count; i++) {
            string id = refinedItem.requiredMaterials[i].ID;
            int amount = refinedItem.requiredQuantities[i];
            InventorySystem.Instance.RemoveItem(id, amount);
        }
        InventorySystem.Instance.AddItem(refinedItem.ID, 1);
        InventorySystem.Instance.discoveredRefinedItems.Add(refinedItem.ID);
        CraftingUIManager.Instance.RefreshInventoryUI();
        refiningUIManager.RefreshRefineUI();
        Debug.Log($"Refined: {refinedItem.itemName}");
    }
    */
    
}
