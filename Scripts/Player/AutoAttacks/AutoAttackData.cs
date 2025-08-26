using UnityEngine;


[System.Serializable]
public enum AutoAttackType
{
    Projectile,
    Aura,
    Spread,
    Shotgun,
    Orbital,
    Area,
}

[CreateAssetMenu(fileName = "AutoAttackData", menuName = "Scriptable Objects/AutoAttackData")]
public class AutoAttackData : ScriptableObject
{
    public GameObject projectilePrefab;
    public float baseAttackCooldown = 5f;
    public int projectileCount;
    public int projectileCount2; // for secondary projectiles or effects
    public float shotInterval = 0.1f;
    public AutoAttackType attackType;
    public float auraRadius = 0f;
}
