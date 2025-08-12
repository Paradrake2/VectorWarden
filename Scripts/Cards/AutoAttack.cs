using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class AutoAttack : MonoBehaviour
{
    public List<PlayerProjectile> projectiles;
    public PlayerStats playerStats;
    public float attackCooldown = 1f; // Default cooldown

    void Start()
    {
        playerStats = PlayerStats.Instance;
        attackCooldown = Mathf.Max(0.1f, 5f - playerStats.GetAutoAttackCooldown());
    }


    // Update is called once per frame
    void Update()
    {
        foreach (var autoAttack in playerStats.autoAttackDataList)
        {
            float cooldown = Mathf.Max(0.1f, autoAttack.baseAttackCooldown - playerStats.GetAutoAttackCooldown());
            float auraCooldown = Mathf.Max(0.05f, playerStats.GetAuraAttackCooldown());
            cooldown -= Time.deltaTime;
            if (cooldown <= 0f)
            {
                if (autoAttack.attackType == AutoAttackType.Aura)
                {
                    cooldown = auraCooldown;
                }
                else
                {
                    cooldown = Mathf.Max(0.1f, autoAttack.baseAttackCooldown - playerStats.GetAutoAttackCooldown());
                }
                StartCoroutine(FireAutoAttack(autoAttack));
            }
        }
    }
    IEnumerator FireAutoAttack(AutoAttackData autoAttack)
    {
        int projectileCount = autoAttack.projectileCount + playerStats.GetAutoAttackProjectileCount();
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
                    yield return new WaitForSeconds(0.1f); // Slight delay between projectiles
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

                break;
            case AutoAttackType.Orbital:
                RotatingAttack(autoAttack, projectileCount);
                break;
            default:
                Debug.LogWarning("Unknown AutoAttackType");
                yield break;
        }
        
        /*
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
        */
    }
    void RotatingAttack(AutoAttackData autoAttack, int projectileCount)
    {
        float damage = playerStats.CurrentDamage * playerStats.GetAutoAttackDamageMultiplier();
        int piercingAmount = Mathf.CeilToInt(damage / 5) + playerStats.GetPiercingAmount(); // how many enemies it can hit before it disappears and has to be regenerated

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
