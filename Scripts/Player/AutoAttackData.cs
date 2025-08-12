using UnityEngine;


[System.Serializable]
public enum AutoAttackType
{
    Projectile,
    Aura,
    Spread,
    Shotgun,
    Orbital
}

[CreateAssetMenu(fileName = "AutoAttackData", menuName = "Scriptable Objects/AutoAttackData")]
public class AutoAttackData : ScriptableObject
{
    public GameObject projectilePrefab;
    public float baseAttackCooldown = 5f;
    public int projectileCount;
    public AutoAttackType attackType;
    public float auraRadius = 0f;
}
