using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class DataManager
{
    private static string savePath = Application.persistentDataPath + "/savefile.json";
    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json); // Overwrites old data
        Debug.Log("save path: " + savePath);
    }

    public static SaveData Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            ApplyData(data);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + savePath);
            return null;
        }
    }
    static void ApplyData(SaveData data)
    {
        if (data == null) return;
        PlayerStats.Instance.BaseHealth = data.maxHealth;
        PlayerStats.Instance.BaseDefense = data.defense;
        PlayerStats.Instance.BaseDamage = data.damage;
        PlayerStats.Instance.BaseGoldGain = data.gold;
        PlayerStats.Instance.BaseXPGain = data.xp;
        try
        {
            foreach (string id in data.unlockedCards)
            {
                CardUnlockNode node = Resources.LoadAll<CardUnlockNode>("CardUnlockNodes").FirstOrDefault(n => n.unlockID == id);
                if (node != null)
                {
                    UnlockState.Instance.NodeUnlock(node);
                }
            }
        }
        catch (NullReferenceException) { }
        try
        {
            InventorySystem.Instance.itemStacks.Clear();
            foreach (InventoryItem item in data.inventoryItems)
            {
                Items itemData = ItemRegistry.Instance.GetItemById(item.itemId);
                if (itemData != null)
                {
                    InventorySystem.Instance.AddItem(itemData.ID, item.quantity);
                }
            }
        }
        catch (NullReferenceException) {
        }
        //PlayerStats.Instance.UnlockedCards = data.unlockedCards;
        //PlayerStats.Instance.InventoryItems = data.inventoryItems;
    }
}
