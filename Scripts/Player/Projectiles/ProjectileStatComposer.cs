using UnityEngine;

public struct ComputedProjectileStats
{
    public GameObject projectilePrefab;
    public float speed;
    public float damage;
    public float size;
    public int pierce;
    public float homingRange;
    public float explosionRadius;
    public float knockbackForce;
    public int projectileAdd;
    public float attackSpeedModifier;
    public bool isHoming;
    public bool isExplosive;

}

public static class ProjectileStatComposer
{
    public static ComputedProjectileStats ComposeStats(ProjectileData baseData)
    {
        var outStats = new ComputedProjectileStats
        {
            projectilePrefab = baseData.prefab,
            speed = baseData.baseSpeed * baseData.speedMultiplier,
            damage = baseData.baseDamage * baseData.damageMultiplier,
            size = baseData.projectileSize,
            pierce = baseData.pierceAmount,
            homingRange = baseData.homingRange,
            explosionRadius = baseData.explosionRadius,
            knockbackForce = baseData.knockbackForce,
            projectileAdd = baseData.projectileAdd,
            attackSpeedModifier = baseData.attackSpeedModifier,
            isHoming = baseData.isHoming,
            isExplosive = baseData.isExplosive
        };

        var tier = ProjectileLevelTracker.Instance.GetTier(baseData);
        if (tier != null)
        {
            if (tier.projectilePrefab != null) outStats.projectilePrefab = tier.projectilePrefab;
            outStats.speed *= tier.speedMultiplier;
            outStats.damage *= tier.damageMultiplier;
            outStats.size *= tier.sizeMult;
            outStats.damage += tier.flatDamage;
            outStats.speed += tier.flatSpeed;
            outStats.explosionRadius += tier.explosionRadius;
            outStats.knockbackForce += tier.knockbackAddition;
            outStats.pierce += tier.piercingAddition;
            outStats.projectileAdd += tier.projectileAdd;
            outStats.homingRange += tier.homingRange;
            outStats.attackSpeedModifier += tier.attackSpeedModifier;
            outStats.isHoming |= tier.unlockHoming;
            outStats.isExplosive |= tier.unlockExplosive;
        }

        return outStats;
    }
}
