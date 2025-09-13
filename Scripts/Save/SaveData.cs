using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class SaveData
{
    public float maxHealth;
    public float damage;
    public float defense;
    public int cardOptions;
    public float gold;
    public float xp;
    public float currentUpgradeXP;
    public List<string> unlockedCardNodes; // list of CardUnlockNode ids that are unlocked
    public List<InventoryItem> inventoryItems; // list of InventoryItems in the player's inventory
    public float musicVolume;
}
