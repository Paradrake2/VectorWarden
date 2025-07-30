using System.Collections.Generic;
using UnityEngine;

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythical,
    Gauranteed
}
public enum ItemType
{
    Scrap,
    Drop,
    Ore,
    Token,
    Material
}
[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Items : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("MaterialColor")]
    public Color color;

    [Header("Stacking")]
    public int maxStackSize = 999;

    [Header("Value")]
    public float value;

    [Header("Rarity")]
    public ItemRarity Rarity;
    [Header("ComponentSlot")]
    public ComponentSlot componentSlot;
    [Header("ItemType")]
    public ItemType itemType;
    [Header("ProjectileType")]
    public ProjectileType projectileType;
    [Header("ItemTier")]
    public float tier;

    [Header("FlatDamage")]
    public float flatDamage;

    [Header("DamageMult")]
    public float damageMult;

    [Header("FlatDefense")]
    public float flatDefense;

    [Header("DefenseMult")]
    public float defenseMult;

    [Header("FlatMaxHP")]
    public float flatMaxHP;

    [Header("MaxHPMult")]
    public float HPMult;

    [Header("FlatPureDamage")]
    public float flatPureDamage;

    [Header("PureDamageMult")]
    public float pureDamageMult;
    [Header("FlatProjectileSpeed")]
    public float flatProjectileSpeed;
    [Header("ProjectileSpeedMult")]
    public float projectileSpeedMult;
    [Header("FlatMaxMana")]
    public float flatMaxMana;

    [Header("MaxManaMult")]
    public float maxManaMult;

    [Header("DropRateIncrease")]
    public float dropRateIncrease;

    [Header("XP gain increase")]
    public float xpGainIncrease;

    [Header("GoldGainIncrease")]
    public float goldGainIncrease;
    [Header("FlatKnockback")]
    public float flatKnockbackIncrease;

    [Header("KnockbackMult")]
    public float knockbackMult;

    [Header("KnockbackResistanceMult")]
    public float knockbackResistanceMult;

    [Header("KnockbackResistanceFlat")]
    public float flatKnockbackResistance;

    [Header("AttackSpeedFlat")]
    public float attackSpeedFlat;
    [Header("AttackSpeedMult")]
    public float attackSpeedMult;

    [Header("Drop Rate")]
    public float dropRate;
    [Header("Dash Distance")]
    public float dashDistance;
    [Header("Dash Number")]
    public int dashNumber;
    [Header("Tags")]
    public string[] tags;
    [Header("Internal ID")]
    public string ID;
    public int coreTier;
    [Header("CoreSize")]
    public int coreSize;
    [Header("Times enhanced")]
    public int enhancedNum;
}
