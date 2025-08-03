using UnityEngine;

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
    public float homingSpeed;
    public float homingRange;
    public float explosionRadius;
    public float pierceAmount;
    public float knockbackForce;
    public float projectileSize;

}