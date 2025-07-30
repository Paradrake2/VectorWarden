using UnityEngine;
using System.Collections.Generic;
public enum EquipmentSlot
{
    Boots,
    Leggings,
    Chestplate,
    Gauntlets,
    Helmet,
    Weapon,
    Accessory
}
public enum WeaponType
{
    Ranged,
    None
}
public enum SlotCategory {
    Armor,
    Weapon,
    Accessory
}
public enum StatType
{
    MaxHealth,
    Defense,
    Damage,
    DropRate,
    GoldGain,
    Regeneration,
    XPGain,
    PureDamage,
    Mana,
    MagicDamage,
    Knockback,
    AttackSpeed,
    ManaRegeneration,
    DashDistance,
    DashNumber,
    ProjectileSpeed,

}

[System.Serializable]
public class StatModifier {
    public StatType statType;
    public float flatAmount = 0f;
    public float percentAmount = 0f;
}

[System.Serializable]
public class Equipment
{
    public string itemName;
    public string ID;
    public Sprite icon;
    public SlotCategory category;
    public EquipmentSlot slot;
    public List<StatModifier> modifiers = new List<StatModifier>();
    //public List<Augment> appliedAugments = new();
    public int augmentSlotNumber;
    public int allowedAugments;
    public int equipmentLevel = 1;
    public float equipmentXP;
    public WeaponType weaponType = WeaponType.None;
    public float attackRange = 0f;
    public float attackSpeed = 1f;
    public float manaCost = 0f;
    public float projectileSpeed = 0f;
    public GameObject projectilePrefab;
    /*
    public int GetXpToNextLevel(Equipment equip) => equip.equipmentLevel * 100;
    public void AddXP(float xp, Equipment equipment) {
        equipmentXP += xp;
        if (equipmentXP >= equipmentLevel*100) {
            equipmentXP = 0;
            equipmentLevel++;
            EquipmentLevelUp(equipment);
        }
    }
    public int XpNeeded(Equipment equipment) {
        return equipment.equipmentLevel * 200;
    }
    public void EnhanceWithPowder(float powderXP, Equipment equipment) {
        AddXP(powderXP, equipment);
    }

    public void EquipmentLevelUp(Equipment equipment) {
        foreach (var mod in equipment.modifiers) {
            if (mod.flatAmount > 0f) {
                mod.percentAmount += 0.01f;
                Debug.Log($"Boosted {mod.statType} + 1% on {equipment.itemName}");
            }
        }
    }
    */
    /*
    public void EquipAugment(Augment augment)
    {
        if (appliedAugments.Contains(augment))
        {
            ApplyAugment.ApplyAugmentToEquipment(augment, this);
        }
        else
        {
            Debug.LogWarning($"Equipment does not have {augment} equipped");
        }
    }
    public void RemoveAugment(Augment augment)
    {
        if (appliedAugments.Contains(augment))
        {
            ApplyAugment.RemoveAugment(appliedAugments.IndexOf(augment), this);
        }
    }
    public bool TryAddAugment(Equipment equipment)
    {
        if (equipment.appliedAugments.Count >= equipment.augmentSlotNumber) return false;
        return true;
    }
    */
    public GameObject GetProjectile()
    {
        return projectilePrefab;
    }
    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }
    public float GetManaCost()
    {
        return manaCost;
    }
    public string GetID()
    {
        return ID;
    }
}
