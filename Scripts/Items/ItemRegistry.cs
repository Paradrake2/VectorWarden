using System.Collections.Generic;
using UnityEngine;

public class ItemRegistry : MonoBehaviour
{
    public static ItemRegistry Instance;

    private Dictionary<string, Items> itemLookup = new();
    //private Dictionary<string, Augment> augmentLookup = new();

    public List<Items> allItems = new(); // Optional for debugging
    //public List<Augment> allAugments = new();
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllItems();
        //LoadAllAugments();
    }

    public void LoadAllItems()
    {
        Items[] items = Resources.LoadAll<Items>("Items");
        allItems = new List<Items>(items);

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.ID))
            {
                itemLookup[item.ID] = item;
            }
            else
            {
                Debug.LogWarning($"Item '{item.name}' has no ID set.");
            }
        }

    }
    public void AddItem(Items item)
    {
        allItems.Add(item);
        Debug.Log($"Added {item.itemName} to item registry");
        itemLookup.Add(item.ID, item);
    }
    /*
    public void LoadAllAugments()
    {
        Augment[] augments = Resources.LoadAll<Augment>("Augments");
        allAugments = new List<Augment>(augments);

        foreach (var augment in augments)
        {
            if (!string.IsNullOrEmpty(augment.augmentId))
            {
                augmentLookup[augment.augmentId] = augment;
            }
            else
            {
                Debug.LogWarning($"Augment '{augment.augmentId}' has no name");
            }
        }
    }
    public Augment GetAugmentById(string id) {
        if (augmentLookup.TryGetValue(id, out var augment)) return augment;
        Debug.LogWarning($"Augment ID '{id}' not found");
        return null;
    }
    */
    public Items GetItemById(string id)
    {
        if (itemLookup.TryGetValue(id, out var item))
            return item;

        Debug.LogWarning($"Item ID '{id}' not found.");
        return null;
    }

}
