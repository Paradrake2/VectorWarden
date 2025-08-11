using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Projectile,
    StatModifier,
    SpecialEffect,
    Random,
    AutoAttack
}
public enum SkillRarity {
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "SkillCard", menuName = "Scriptable Objects/SkillCard")]
public class SkillCard : ScriptableObject
{
    public string skillName;
    public string description;
    public Sprite skillIcon; // the icon that will show up in the UI
    public Sprite cardPicture;
    public SkillType skillType;
    public SkillRarity skillCardRarity;
    public List<StatModifier> statModifiers;
    public GameObject projectilePrefab;
    public int minimumLevelRequired = 1;
    public List<SpecialEffects> specialEffects; // List of special effects that this skill card can apply
    public ProjectileData projectileData; // Reference to the projectile data if this is a projectile skill
    public bool unlocked = false;

    [Header("Random Card Settings")]
    public float maxValue;
    public float minValue;

    public List<SkillCard> prerequisites;
}


