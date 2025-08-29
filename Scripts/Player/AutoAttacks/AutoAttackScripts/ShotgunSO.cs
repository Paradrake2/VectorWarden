using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AutoAttack/Shotgun")]

public class ShotgunSO : AutoAttackData
{
    public int startPelletCount = 3;
    public override IEnumerator Execute(AutoAttackContext ctx)
    {
        // Implementation for the shotgun attack
        var ps = ctx.PlayerStats;
        int level = ProjectileLevelTracker.Instance.GetLevel(projectilePrefab.GetComponent<PlayerProjectile>().projectileData);
        var data = projectilePrefab.GetComponent<PlayerProjectile>().projectileData;
        var projUp = data?.projectileUpgrade;

        int pelletCount = startPelletCount+(projUp?.tiers[level].projectileAdd ?? 0);
        float spreadAngle = 3.3f * pelletCount; // Total spread angle in degrees

        var target = ctx.GetNearestEnemy(ctx.Origin.position);
        if (target == null) yield break; // no valid targets

        Vector3 directionToTarget = (target.position - ctx.Origin.position).normalized;
        float angleStep = spreadAngle / (pelletCount - 1);
        float startAngle = -spreadAngle / 2;

        for (int i = 0; i < pelletCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.AngleAxis(currentAngle, Vector3.up);
            Vector3 pelletDirection = rotation * directionToTarget;

            ctx.FireProjectile(pelletDirection, projectilePrefab, ctx.Origin.position);
        }

        yield break;
    }
}
