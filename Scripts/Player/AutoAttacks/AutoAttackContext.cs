using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoAttackContext
{
    public Transform Origin { get; }
    public PlayerStats PlayerStats { get; }
    public MonoBehaviour Runner { get; }
    public IEnemyQuery EnemyQuery { get; }
    public AutoAttackData attackData { get; }
    public AutoAttackContext(Transform origin, PlayerStats playerStats, MonoBehaviour runner, IEnemyQuery enemyQuery, AutoAttackData attackData)
    {
        Origin = origin;
        PlayerStats = playerStats;
        Runner = runner;
        EnemyQuery = enemyQuery;
        this.attackData = attackData;
    }

    // Helpers
    public GameObject FireProjectile(Vector3 direction, GameObject prefab, Vector3 position)
    {
        var go = Object.Instantiate(prefab, position, Quaternion.identity);
        var proj = go.GetComponent<PlayerProjectile>();
        proj?.InitializeProjectile(direction, PlayerStats.GetProjectileSpeed(), PlayerStats.GetAutoAttackDamage(), proj.projectileType, proj.projectileData, PlayerStats);
        return go;
    }
    public Transform GetNearestEnemy(Vector3 from)
    {
        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in EnemyQuery.GetAllEnemies())
        {
            float distance = Vector3.Distance(from, enemy.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }

        return nearest;
    }

    public Camera MainCamera => Camera.main; // for screen scrolling attacks
    public void DamageEnemy(Transform enemy, float damage)
    {
        enemy.GetComponent<EnemyStats>()?.TakeDamage(damage);
    }

    // Probably wont be used
    public void HealPlayer(float amount)
    {
        PlayerStats.GainHealth(amount);
    }

    public GameObject SpawnStationaryProjectile(Vector3 direction, GameObject prefab, Vector3 position, float time,Vector3 offset = default, float rotation = 0f)
    {
        var go = Object.Instantiate(prefab, position, Quaternion.identity);
        var proj = go.GetComponent<PlayerProjectile>();
        proj?.InitializeProjectile(direction, 0, PlayerStats.GetAutoAttackDamage(), proj.projectileType, proj.projectileData, proj.playerStats);
        Vector3 rotatedOffset = Quaternion.Euler(0, 0, rotation) * offset;
        go.transform.position += rotatedOffset;
        go.transform.rotation = Quaternion.Euler(0, 0, rotation);
        return go;
    }

    public List<Transform> EnemiesInRadius(Vector3 center, float radius) => EnemyQuery.GetAllEnemies().Where(t => (t.position - center).sqrMagnitude <= radius * radius).ToList();
}
