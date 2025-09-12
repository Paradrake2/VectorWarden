using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

// -------------------------------- DEPRECATED --------------------------
public enum EquipmentSlot
{
    Weapon,
    Accessory
}
[System.Serializable]
public class StatModifier
{
    public StatType statType;
    public float flatAmount = 0f;
    public float percentAmount = 0f;
}
[System.Serializable]
public class Equipment
{
    public EquipmentSlot slot;
    public string equipmentName;
    public Sprite icon;
    public int ID;
    public float baseDamage;
    public float baseDefense;
    public float baseHealth;
    public float baseAttackSpeed;
    public ProjectileType projectileType = ProjectileType.None;
    public float baseProjectileSpeed;
    public GameObject projectilePrefab = null; // Prefab for the projectile if this equipment is a weapon

    public List<StatModifier> modifiers = new List<StatModifier>();

    public Equipment(EquipmentSlot slot, string equipmentName, Sprite icon, int ID, float baseDamage, float baseDefense, float baseHealth, float baseAttackSpeed, ProjectileType projectileType = ProjectileType.None, float baseProjectileSpeed = 0f)
    {
        this.slot = slot;
        this.equipmentName = equipmentName;
        this.icon = icon;
        this.ID = ID;
        this.baseDamage = baseDamage;
        this.baseDefense = baseDefense;
        this.baseHealth = baseHealth;
        this.baseAttackSpeed = baseAttackSpeed;
        this.projectileType = projectileType;
        this.baseProjectileSpeed = baseProjectileSpeed;
    }

    // Additional properties can be added as needed
    public float GetDamage()
    {
        return baseDamage;
    }

    public float GetDefense()
    {
        return baseDefense;
    }

    public float GetProjectileSpeed()
    {
        return baseProjectileSpeed;
    }
    public string GetID()
    {
        return ID.ToString();
    }
}
