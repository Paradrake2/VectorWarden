using System.Collections;
using UnityEngine;

/*
// ------------ DEPRECATED ---------------
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
*/
[CreateAssetMenu(fileName = "AutoAttackData", menuName = "Scriptable Objects/AutoAttackData")]
public abstract class AutoAttackData : ScriptableObject
{
    public GameObject projectilePrefab;
    public float baseAttackCooldown = 5f;
    // public AutoAttackType attackType;
    //public float auraRadius = 0f;
    public abstract IEnumerator Execute(AutoAttackContext ctx);
    
}
