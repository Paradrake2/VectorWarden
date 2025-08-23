using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileUpgrade", menuName = "Scriptable Objects/ProjectileUpgrade")]
public class ProjectileUpgrade : ScriptableObject
{
    public ProjectileData projectileData;
    public int maxLevel = 7;

    public List<Tier> tiers = new List<Tier>();

    [System.Serializable]
    public class Tier
    {
        public GameObject projectilePrefab;
        [Header("Stat multipliers")]
        public float damageMultiplier = 1f;
        public float speedMultiplier = 1f;
        public float sizeMult = 1f;

        [Header("Flat Additions")]
        public float flatDamage = 0f;
        public float flatSpeed = 0f;
        public float explosionRadius = 0f;
        public float knockbackAddition = 0f;
        public int piercingAddition = 0;
        public int projectileAdd = 0;
        public float homingRange = 0f;
        public float attackSpeedModifier = 0f;

        [Header("Unlocks / Flags")]
        public bool unlockExplosive = false;
        public bool unlockHoming = false;
        public bool shotgunStyle = false;

        [Header("Custom Hooks")]
        public GameObject onHitVFX;
        public AudioClip onHitSFX;
        [Header("Misc Stats")]
        public float float1;
        public float float2;
        public float float3;
        public float float4;
        public int int1;
        public int int2;
        public int int3;
        public int int4;
    }

    public Tier GetTier(int level)
    {
        int idx = Mathf.Clamp(level - 1, 0, tiers.Count - 1);
        return tiers.Count > 0 ? tiers[idx] : null;
    }
}
