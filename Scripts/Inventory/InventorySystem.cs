using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventorySystem : MonoBehaviour
{
    public List<InventoryItem> itemStacks = new List<InventoryItem>();
    public List<InventoryItem> acquiredItems = new List<InventoryItem>(); // Stuff the player has gotten during the run

    //public List<Augment> ownedAugments = new List<Augment>();
    public HashSet<String> discoveredRefinedItems = new();
    public static InventorySystem Instance;
    //public Log log;
    public void RemoveItem(string itemId, int amount)
    {
        var stack = itemStacks.Find(i => i.itemId == itemId);
        if (stack != null)
        {
            stack.quantity -= amount;
            if (stack.quantity <= 0)
            {
                itemStacks.Remove(stack);
            }
        }
    }
    public void RemoveItemsWithTag(string tag, int quantity)
    {
        for (int i = 0; i < itemStacks.Count && quantity > 0; i++)
        {
            var entry = itemStacks[i];
            Items item = ItemRegistry.Instance.GetItemById(entry.itemId);
            if (item != null && item.tags.Contains(tag))
            {
                int toRemove = Mathf.Min(entry.quantity, quantity);
                entry.quantity -= toRemove;
                quantity -= toRemove;

                if (entry.quantity <= 0)
                {
                    itemStacks.RemoveAt(i);
                    i--;
                }
            }
        }
        if (quantity > 0)
        {
            Debug.LogWarning($"Tried to remove more items with tag {tag} than were available");
        }
    }
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Dungeon")
        {
            //log = FindFirstObjectByType<Log>();
        }
    }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }
    /*
    public void AddAugment(Augment newAugment)
    {
        ownedAugments.Add(newAugment);
        Debug.Log($"Added new augment: {newAugment.augmentName}");
    }
    public void RemoveAugment(Augment augment)
    {
        ownedAugments.Remove(augment);
        Debug.Log($"Removed {augment}");
    }
    */
    public void AddEquipment(Equipment newEquip)
    {
        PlayerStats.Instance.ownedGear.Add(newEquip);
        PlayerStats.Instance.existingGear.Add(newEquip);
        Debug.Log($"Added new equipment: {newEquip.equipmentName}");
    }
    public void AddItem(string itemId, int amount)
    {
        InventoryItem existing = itemStacks.Find(i => i.itemId == itemId);

        if (existing != null)
        {
            existing.quantity += amount;
        }
        else
        {
            itemStacks.Add(new InventoryItem { itemId = itemId, quantity = amount });
        }
        if (SceneManager.GetActiveScene().name == "Dungeon")
        {
            //if (log == null) log = FindFirstObjectByType<Log>();
            //if (log != null) log.AddLogMessage(itemId, amount);
        }

    }
    public void AddItemToSpoils(string itemId, int amount)
    {
        InventoryItem existing = acquiredItems.Find(i => i.itemId == itemId);

        if (existing != null)
        {
            existing.quantity += amount;
        }
        else
        {
            acquiredItems.Add(new InventoryItem { itemId = itemId, quantity = amount });
        }
    }
    public bool HasItemsWithTag(string tag, int requiredAmount)
    {
        int total = 0;
        foreach (var entry in InventorySystem.Instance.itemStacks)
        {
            Items item = ItemRegistry.Instance.GetItemById(entry.itemId);
            if (item != null && item.tags.Contains(tag))
            {
                total += entry.quantity;
                if (total >= requiredAmount) return true;
            }
        }
        return false;
    }

    public bool HasItem(string itemId, int amount)
    {
        var stack = itemStacks.Find(i => i.itemId == itemId);
        return stack != null && stack.quantity >= amount;
    }
    /*
    public Augment GetAugment(string augmentName)
    {
        foreach (var augment in ownedAugments)
        {
            if (augment.augmentName == augmentName)
            {
                return augment;
            }
        }
        return null;
    }
    */
    public int GetQuantity(string requestedItemName)
    {
        foreach (var item in itemStacks)
        {
            if (item.itemId == requestedItemName)
            {
                return item.quantity;
            }
        }
        return 0;
    }
    /*
    public int GetMaxRefinableAmount(RefineRecipe recipe)
    {
        int min = int.MaxValue;
        foreach (var req in recipe.requirements)
        {
            int totalHave = GetTotalItemCountByTag(req.requiredTag);
            int maxForThisTag = totalHave / req.quantityRequired;
            min = Mathf.Min(min, maxForThisTag);
        }
        return Mathf.Clamp(min, 0, 99999);
    }
    */

    public int GetTotalItemCountByTag(string tag)
    {
        int total = 0;
        foreach (var stack in itemStacks)
        {
            Items item = ItemRegistry.Instance.GetItemById(stack.itemId);
            if (item.tags.Contains(tag))
            {
                total += stack.quantity;
            }
        }
        return total;
    }
}
