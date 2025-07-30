using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    MaxHealth,
    Defense,
    Damage,
    DropRate,
    Mana,
    Knockback,
    GoldGain,
    Regeneration,
    XPGain,
    ManaRegeneration,
    DashDistance,
    DashNumber,
    ProjectileSpeed,
    AttackSpeed
}
public class PlayerStats : MonoBehaviour
{
    public Dictionary<EquipmentSlot, Equipment> equippedItems = new();
    public List<Equipment> ownedGear = new List<Equipment>();
    public List<Equipment> existingGear = new List<Equipment>();
    public Equipment[] accessorySlots;
    public static PlayerStats Instance;
    public int accessorySlotNum = 5;
    public int Level = 1;
    public float XP = 0;
    public float BaseHealth = 100;
    public float BaseDefense = 0;
    public float BaseDamage = 1;
    public float PureDamage = 0;
    public float BaseMana = 100;
    public float BaseKnockback = 1;
    public float BaseAttackSpeed = 0.5f;
    public float BaseDropRate = 0.5f;
    public float BaseXPGain = 1f;
    public float BaseGoldGain = 1f;
    public float BaseRegeneration = 0f;
    public float BaseManaRegeneration = 1f;
    public float BaseProjectileSpeed = 20f;
    public float CurrentHealth;
    public float DashDistance = 2f;
    public int DashNumber = 2;
    public bool isDashing = false;


    public float CurrentDefense => CalculateStat(StatType.Defense);
    public float CurrentDamage => CalculateStat(StatType.Damage) + PureDamage;
    public float CurrentMaxHealth => CalculateStat(StatType.MaxHealth);
    public float CurrentMaxMana => BaseMana;
    public float CurrentMagicDamage => 0f;
    public float CurrentKnockback => CalculateStat(StatType.Knockback);
    public float CurrentAttackSpeed => CalculateStat(StatType.AttackSpeed);
    public float CurrentRegeneration => BaseRegeneration;
    public float CurrentManaRegeneration => BaseManaRegeneration;
    public float CurrentProjectileSpeed => CalculateStat(StatType.ProjectileSpeed);
    public float CurrentDropRate => BaseDropRate;
    public float CurrentXPGain => CalculateStat(StatType.XPGain);
    public float CalculateDashDistance => CalculateStat(StatType.DashDistance);
    public int CalculateDashNumber => DashNumber;


    public float XpToNextLevel => Level * 1000;
    public float CurrentMana;
    public int skillPoints;

    public ProjectileType CurrentProjectileType = ProjectileType.Normal;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        accessorySlots = new Equipment[accessorySlotNum];
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        CurrentHealth = CurrentMaxHealth;
        CurrentMana = CurrentMaxMana;
    }

    public void GainXP(float amount)
    {
        XP += amount;
        while (XP >= XpToNextLevel)
        {
            XP -= XpToNextLevel;
            LevelUp();
        }
    }
    void LevelUp()
    {
        Level++;
        BaseHealth += 10;
        BaseDefense += 0.5f;
        BaseMana += 5f;
        skillPoints++;
        if (Level % 5 == 0)
        {
            BaseDamage += 3f;
            BaseManaRegeneration += 1f;
        }
        Debug.Log("Leveled up to level " + Level);
    }
    public float GetStat(StatType statType)
    {
        return CalculateStat(statType);
    }
    float CalculateStat(StatType type)
    {
        float flat = 0f;
        float percent = 1f;
        
        foreach (var kvp in equippedItems)
        {
            var item = kvp.Value;
            if (item == null) continue;
            foreach (var mod in item.modifiers)
            {
                if (mod.statType == type)
                {
                    flat += mod.flatAmount;
                    percent += mod.percentAmount;
                }
            }
        }
        foreach (var accessory in accessorySlots)
        {
            if (accessory == null) continue;
            foreach (var mod in accessory.modifiers)
            {
                if (mod.statType == type)
                {
                    flat += mod.flatAmount;
                    percent += mod.percentAmount;
                }
            }
        }
        float baseValue = type switch
        {
            StatType.MaxHealth => BaseHealth,
            StatType.Defense => BaseDefense,
            StatType.Damage => BaseDamage,
            StatType.DropRate => BaseDropRate,
            StatType.Mana => BaseMana,
            StatType.Knockback => BaseKnockback,
            StatType.GoldGain => BaseGoldGain,
            StatType.Regeneration => BaseRegeneration,
            StatType.XPGain => BaseXPGain,
            StatType.ManaRegeneration => BaseManaRegeneration,
            StatType.DashDistance => DashDistance,
            StatType.DashNumber => DashNumber,
            StatType.ProjectileSpeed => BaseProjectileSpeed,
            StatType.AttackSpeed => BaseAttackSpeed,
            _ => 0
        };
        return (baseValue + flat) * percent;
    }
}
