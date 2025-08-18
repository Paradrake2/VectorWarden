using UnityEngine;

public enum ProjectileOrigin
{
    ManualOnly,
    AutoOnly,
    OrbitalOnly,
    ManualAndAuto,
    Any
}

[CreateAssetMenu(menuName = "Scriptable Objects/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public string projectileName;
    public GameObject prefab;
    public float baseSpeed;
    public float baseDamage;
    public Sprite icon;
    public ProjectileType projectileType;
    public float spriteRotationOffset = 0f; // Offset to align the sprite correctly
    public float lifetime = 5f;

    [Header("Projectile Settings")]
    public float speedMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float attackSpeedModifier = 0f; // Modifier for attack speed
    public bool isHoming;
    public bool isExplosive;
    public bool shotgunStyle;
    public float homingSpeed;
    public float homingRange;
    public float explosionRadius;
    public int pierceAmount;
    public float knockbackForce;
    public float projectileSize;
    public int projectileAdd = 0; // Number of additional projectiles to spawn

    public ProjectileUpgrade projectileUpgrade; // Reference to the upgrade data
    public ProjectileOrigin projectileOrigin = ProjectileOrigin.Any;
}