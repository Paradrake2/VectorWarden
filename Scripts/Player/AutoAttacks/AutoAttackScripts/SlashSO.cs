using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AutoAttack/Slash")]
public class SlashSO : AutoAttackData
{
    public float slashDistance;
    public float slashTime;
    public override IEnumerator Execute(AutoAttackContext ctx)
    {
        Transform nearestEnemy = ctx.GetNearestEnemy(ctx.Origin.position);
        float distanceToEnemy = Vector3.Distance(ctx.Origin.position, nearestEnemy.position);
        if (distanceToEnemy <= slashDistance)
        {
            ctx.SpawnStationaryProjectile(ctx.GetNearestEnemy(ctx.Origin.position).position, projectilePrefab, ctx.Origin.position, 5);
        }
        yield return null;
    }
}
