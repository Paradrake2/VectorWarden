using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class AutoAttack : MonoBehaviour
{
    public List<PlayerProjectile> projectiles;
    public PlayerStats playerStats;
    public float attackCooldown = 1f; // Default cooldown
    private float attackTimer = 0f;

    void Start()
    {
        playerStats = PlayerStats.Instance;
        attackCooldown = Mathf.Max(0.1f, 5f - playerStats.GetAutoAttackCooldown());
    }


    // Update is called once per frame
    void Update()
    {
        if (playerStats.HasAutoAttack())
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                StartCoroutine(FireAutoAttack());
                attackTimer = playerStats.GetAutoAttackCooldown();
            }
        }
    }
    IEnumerator FireAutoAttack()
    {
        var projectilePrefabs = playerStats.GetAutoAttackProjectiles();
        int projectileCount = playerStats.GetAutoAttackProjectileCount();
        float attackDelay = playerStats.GetAutoAttackCooldown();

        List<GameObject> selectedProjectiles = new List<GameObject>();
        for (int i = 0; i < projectileCount; i++)
        {
            GameObject randomProjectile = projectilePrefabs[Random.Range(0, projectilePrefabs.Count)];
            selectedProjectiles.Add(randomProjectile);
        }
        foreach (var prefab in selectedProjectiles)
        {
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 direction = (GetNearestEnemy().transform.position - transform.position).normalized;
                FireProjectile(direction, prefab);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    void FireProjectile(Vector3 direction, GameObject prefab)
    {
        GameObject projectile = Instantiate(prefab, transform.position, Quaternion.identity);
        PlayerProjectile projData = projectile.GetComponent<PlayerProjectile>();
        if (projData != null)
        {
            projData.InitializeProjectile(direction, playerStats.GetProjectileSpeed(), playerStats.CurrentDamage * playerStats.GetAutoAttackDamageMultiplier(), projData.projectileType);
        }
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * playerStats.GetProjectileSpeed();
        }
    }

    GameObject GetNearestEnemy()
    {
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        List<EnemyStats> enemies = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None).ToList();
    
        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.gameObject.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy != null ? closestEnemy.gameObject : null;
    }
}
