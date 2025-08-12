using UnityEngine;

public enum SpecialEffectType
{
    None,
    AutoFire,
    MultipleProjectiles,
    Lifesteal,
    CriticalChance,
    CriticalDamage,
    ExplosionRadius,
    Piercing,
    HomingRange,
    ProjectileSpeed,
    ProjectileSize,
    AutoAttackCooldown,
    AutoAttackProjectileCount,
    AutoAttackDamageMult,
    CardOptions,
    AuraDamageMult,
    AuraRadius,
    AuraAttackCooldown,

    // other stuff
}

[CreateAssetMenu(fileName = "SpecialEffects", menuName = "Scriptable Objects/SpecialEffects")]
public class SpecialEffects : ScriptableObject
{
    public SpecialEffectType effectType;
    public float value;
}
