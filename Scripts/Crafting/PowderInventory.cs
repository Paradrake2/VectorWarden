using UnityEngine;

public class PowderInventory : MonoBehaviour
{
    public static PowderInventory Instance;
    public int totalPowder;
    void Awake()
    {
        Instance = this;
    }
    public void CoreToPowder(Items core) {
        int amountToConvert = 1;
        int mult = 5; // eventually maybe add an upgrade that increases this mult?
        AddPowder(core.coreSize * core.coreTier * mult * amountToConvert);
        InventorySystem.Instance.RemoveItem(core.itemName, 1);
    }
    public void AddPowder(int amount) {
        totalPowder += amount;
        Debug.Log($"+{amount} Powder (total: {totalPowder})");
    }
    public void RemovePowder(int amount) {
        totalPowder -= amount;
        Debug.Log($"-{amount} Powder (total: {totalPowder})");
    }
    public bool EnoughPowder(int amount) {
        if (totalPowder >= amount) return true;
        return false;
    }
    public int getPowder() {
        return totalPowder;
    }
}
