using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "AutoAttack/Projectile")]
public class ProjectileAttackSO : AutoAttackData
{
    public int startProjCount = 1;
    public float delayBetweenProjectiles = 0.08f;
    public override IEnumerator Execute(AutoAttackContext ctx)
    {
        // Implementation for the projectile attack
        var ps = ctx.PlayerStats;
        int level = ProjectileLevelTracker.Instance.GetLevel(projectilePrefab.GetComponent<PlayerProjectile>().projectileData);
        var data = projectilePrefab.GetComponent<PlayerProjectile>().projectileData;
        var projUp = data?.projectileUpgrade;

        int projectileCount = startProjCount + (projUp?.tiers[level].projectileAdd ?? 0);

        for (int i = 0; i < projectileCount; i++)
        {
            var target = ctx.GetNearestEnemy(ctx.Origin.position);
            if (target == null) continue; // no valid targets
            Vector3 direction = target.position - ctx.Origin.position;
            ctx.FireProjectile(direction, projectilePrefab, ctx.Origin.position);
            yield return new WaitForSeconds(delayBetweenProjectiles);
        }
    }
}
