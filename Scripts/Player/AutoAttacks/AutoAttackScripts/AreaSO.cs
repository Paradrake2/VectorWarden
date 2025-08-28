using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaSO : AutoAttackData
{
    public float attackRadius = 3f;
    public int startProjectileCount = 3;
    public float cooldown = 0f;
    public override IEnumerator Execute(AutoAttackContext ctx)
    {
        var enemies = ctx.EnemyQuery.GetAllEnemies().ToList();
        List<Vector3> spawnPos = new List<Vector3>();
        int projectileCount = startProjectileCount + projectilePrefab.GetComponent<PlayerProjectile>().projectileData.projectileUpgrade.tiers[ProjectileLevelTracker.Instance.GetLevel(projectilePrefab.GetComponent<PlayerProjectile>().projectileData)].projectileAdd;

        for (int i = 0; i < projectileCount; i++)
        {
            if (enemies.Count() == 0) break;
            var enemy = enemies[Random.Range(0, enemies.Count())];
            enemies.Remove(enemy);
            spawnPos.Add(enemy.position);
        }

        foreach (Vector3 pos in spawnPos)
        {
            ctx.FireProjectile(Vector3.zero, projectilePrefab, pos);
            yield return new WaitForSeconds(cooldown);
        }
        Debug.LogError("AreaSO Execute not implemented yet.");
        yield break;
    }
}
