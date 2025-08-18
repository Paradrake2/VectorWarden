using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class AutoAttack : MonoBehaviour
{
    public List<PlayerProjectile> projectiles;
    public PlayerStats playerStats;
    public float attackCooldown = 1f; // Default cooldown

    Dictionary<AutoAttackData, float> attackTimers = new Dictionary<AutoAttackData, float>();
    void Start()
    {
        playerStats = PlayerStats.Instance;
        //attackCooldown = Mathf.Max(0.1f, 5f - playerStats.GetAutoAttackCooldown());
    }


    // Update is called once per frame
    void Update()
    {
        foreach (var autoAttack in playerStats.autoAttackDataList)
        {
            if (!attackTimers.ContainsKey(autoAttack))
            {
                attackTimers[autoAttack] = 0f;
            }
            attackTimers[autoAttack] -= Time.deltaTime;
            if (attackTimers[autoAttack] <= 0f)
            {
                StartCoroutine(FireAutoAttack(autoAttack));
                float cd = Mathf.Max(0.1f, playerStats.GetAutoAttackCooldown(autoAttack));
                attackTimers[autoAttack] = cd; // Reset the timer
            }
        }
    }
    IEnumerator FireAutoAttack(AutoAttackData autoAttack)
    {
        int level = ProjectileLevelTracker.Instance.GetLevel(autoAttack.projectilePrefab.GetComponent<PlayerProjectile>().projectileData);
        var pp = autoAttack.projectilePrefab.GetComponent<PlayerProjectile>();
        var data = pp?.projectileData;
        ProjectileUpgrade projUp = data?.projectileUpgrade;


        int projectileCount = playerStats.GetAutoAttackProjectileCount(autoAttack) + (projUp?.tiers[level].projectileAdd ?? 0) + 1; // +1 for the base projectile
        Debug.Log($"Firing AutoAttack: {autoAttack.attackType} with {projectileCount} projectiles");
        switch (autoAttack.attackType)
        {
            case AutoAttackType.Projectile:
                for (int i = 0; i < projectileCount; i++)
                {
                    GameObject nearestEnemy = GetNearestEnemy();
                    if (nearestEnemy != null)
                    {
                        Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
                        FireProjectile(direction, autoAttack.projectilePrefab);
                    }
                    yield return new WaitForSeconds(0.05f); // Slight delay between projectiles
                }
                break;
            case AutoAttackType.Aura:
                float auraRadius = autoAttack.auraRadius;
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, auraRadius);
                // create aura effect
                foreach (var enemy in hitEnemies)
                {
                    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                    if (enemyStats != null)
                    {
                        enemyStats.TakeDamage(playerStats.CurrentDamage * playerStats.GetAuraDamageMult());
                    }
                }
                break;
            case AutoAttackType.Spread:
                float angleStep = 360f / projectileCount;
                float currentAngle = 0f;

                for (int i = 0; i < projectileCount; i++)
                {
                    float radian = currentAngle * Mathf.Deg2Rad;
                    Vector3 direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0).normalized;
                    FireProjectile(direction, autoAttack.projectilePrefab);
                    currentAngle += angleStep;
                }
                break;
            case AutoAttackType.Shotgun:
                Shotgun(projectileCount, autoAttack);
                break;
            case AutoAttackType.Orbital:
                //RotatingAttack(autoAttack, projectileCount);
                break;
            default:
                Debug.LogWarning("Unknown AutoAttackType");
                yield break;
        }
        
    }

    
    void Shotgun(int projectileCount, AutoAttackData autoAttack)
    {
        GameObject nearestEnemy = GetNearestEnemy();
        float spread = 3.3f * projectileCount;
        for (int i = 0; i < projectileCount; i++)
        {
            float randomAngle = Random.Range(-spread / 2, spread / 2);
            Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);
            Vector2 rotatedDirection = rotation * (nearestEnemy.transform.position - transform.position).normalized;
            FireProjectile(rotatedDirection, autoAttack.projectilePrefab);
        }
    }
    void FireProjectile(Vector3 direction, GameObject prefab)
    {
        GameObject projectile = Instantiate(prefab, transform.position, Quaternion.identity);
        PlayerProjectile projData = projectile.GetComponent<PlayerProjectile>();
        
        if (projData != null)
        {
            projData.InitializeProjectile(direction, playerStats.GetProjectileSpeed(), playerStats.CurrentDamage * playerStats.GetAutoAttackDamageMultiplier(), projData.projectileType, projData.projectileData, playerStats);
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
