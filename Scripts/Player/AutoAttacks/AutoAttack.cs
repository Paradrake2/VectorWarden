using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




public class AutoAttack : MonoBehaviour
{
    public List<PlayerProjectile> projectiles;
    public PlayerStats playerStats;
    public float attackCooldown = 1f; // Default cooldown
    private IEnemyQuery enemyQuery;
    Dictionary<AutoAttackData, float> attackTimers = new Dictionary<AutoAttackData, float>(); // per attack cooldown trackers
    void Start()
    {
        playerStats = PlayerStats.Instance;
        //attackCooldown = Mathf.Max(0.1f, 5f - playerStats.GetAutoAttackCooldown());
        enemyQuery = new SceneEnemyQuery();
    }

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
                StartCoroutine(autoAttack.Execute(new AutoAttackContext(transform, playerStats, this, enemyQuery, autoAttack)));
                float cd = Mathf.Max(0.1f, playerStats.GetAutoAttackCooldown(autoAttack));
                attackTimers[autoAttack] = cd; // Reset the timer
            }
        }
    }

    /*
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
    public IEnumerator FireAutoAttack(AutoAttackData autoAttack)
    {
        int level = ProjectileLevelTracker.Instance.GetLevel(autoAttack.projectilePrefab.GetComponent<PlayerProjectile>().projectileData);
        var pp = autoAttack.projectilePrefab.GetComponent<PlayerProjectile>();
        var data = pp?.projectileData;
        ProjectileUpgrade projUp = data?.projectileUpgrade;


        int projectileCount = playerStats.GetAutoAttackProjectileCount(autoAttack) + (projUp?.tiers[level].projectileAdd ?? 0) + 1; // +1 for the base projectile
        //Debug.Log($"Firing AutoAttack: {autoAttack.attackType} with {projectileCount} projectiles");
        switch (autoAttack.attackType)
        {
            case AutoAttackType.Projectile: // Basic auto attack, fires a projectile at the closest enemy
                for (int i = 0; i < projectileCount; i++)
                {
                    GameObject nearestEnemy = GetNearestEnemy();
                    if (nearestEnemy != null)
                    {
                        Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
                        FireProjectile(direction, autoAttack.projectilePrefab, transform.position);
                    }
                    yield return new WaitForSeconds(0.05f); // Slight delay between projectiles
                }
                break;
            case AutoAttackType.Aura:
                AuraAttack(autoAttack);
                break;
            case AutoAttackType.Spread:
                Spread(autoAttack.projectileCount, autoAttack.projectileCount2, autoAttack.shotInterval, autoAttack.projectilePrefab);
                break;
            case AutoAttackType.Shotgun:
                Shotgun(projectileCount, autoAttack);
                break;
            case AutoAttackType.Orbital:
                // This is handled in the OrbitalAttack class. It has its own logic because there is a lot more that goes into it that simply firing a projectile. This is just here as a placeholder
                break;
            case AutoAttackType.Area:
                AreaAttack(autoAttack);
                break;
            default:
                Debug.LogWarning("Unknown AutoAttackType");
                yield break;
        }
    }
    void AreaAttack(AutoAttackData aa)
    {
        EnemyStats[] enemies = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);
        List<Vector3> spawnPos = new List<Vector3>();
        for (int i = 0; i < aa.projectilePrefab.GetComponent<PlayerProjectile>().projectileData.projectileAdd; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemies.Length);
            Vector3 enemyPosition = enemies[randomIndex].transform.position;
            if (!spawnPos.Contains(enemyPosition))
            {
                spawnPos.Add(enemyPosition);
                Debug.Log(enemyPosition);
            }
        }
        foreach (Vector3 pos in spawnPos)
        {
            FireProjectile(pos, aa.projectilePrefab, pos);
        }
    }
    void AuraAttack(AutoAttackData aa)
    {
        float auraRadius = aa.auraRadius;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, auraRadius);
        StartCoroutine(ShowAura(aa.projectilePrefab, auraRadius));
        foreach (var enemy in hitEnemies)
        {
            Debug.Log($"Aura hit enemy: {enemy.name}");
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(playerStats.CurrentDamage);
                Vector3 popupPosition = enemy.transform.position + Vector3.up * 1.2f;
                DamagePopup.Spawn(playerStats.CurrentDamage, popupPosition, Color.red);
            }
        }
    }
    IEnumerator ShowAura(GameObject prefab, float radius)
    {
        Player player = FindFirstObjectByType<Player>();
        GameObject aura = Instantiate(prefab, player.transform.position, Quaternion.identity);
        aura.transform.localScale = new Vector3(radius, radius, 1);
        yield return new WaitForSeconds(0.2f);
        Destroy(aura);
    }
    void Spread(int streams, int shotsPerStream, float perShotInterval, GameObject prefab)
    {
        float angleStep = 360f / Mathf.Max(1, streams);
        float currentAngle = 45f;

        for (int i = 0; i < streams; i++)
        {
            float radian = currentAngle * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0).normalized;
            StartCoroutine(FireStream(direction, shotsPerStream, perShotInterval, prefab));
            //FireProjectile(direction, autoAttack.projectilePrefab);
            currentAngle += angleStep;
        }
    }
    IEnumerator FireStream(Vector2 dir, int count, float interval, GameObject prefab)
    {
        for (int i = 0; i < count; i++)
        {
            FireProjectile(dir, prefab, transform.position);
            yield return new WaitForSeconds(interval);
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
            FireProjectile(rotatedDirection, autoAttack.projectilePrefab, transform.position);
        }
    }
    void FireProjectile(Vector3 direction, GameObject prefab, Vector3 position)
    {
        GameObject projectile = Instantiate(prefab, position, Quaternion.identity);
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
    /*
    public void AreaAttack()
    {
        EnemyStats[] enemies = FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);
        List<Vector3> spawnPos = new List<Vector3>();
        for (int i = 0; i < projectileData.projectileAdd; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, enemies.Length);
            Vector3 enemyPosition = enemies[randomIndex].transform.position;
            if (!spawnPos.Contains(enemyPosition))
            {
                spawnPos.Add(enemyPosition);
                Debug.Log(enemyPosition);
            }
        }
        foreach (Vector3 pos in spawnPos)
        {
            StartCoroutine(SpawnAreaAttack(pos));
        }
    }

    private IEnumerator SpawnAreaAttack(Vector3 position)
    {
        GameObject projectile = Instantiate(projectileData.prefab, position, Quaternion.identity);
        PlayerProjectile projData = projectile.GetComponent<PlayerProjectile>();
        SetupAreaAttackStats(projData, projectile);
        Destroy(projectile, projectileData.float1);
        yield return null;
    }

    private void SetupAreaAttackStats(PlayerProjectile projData, GameObject projectile)
    {
        projData.damage = damage;
        projectile.transform.localScale = new Vector3(projectileSize, projectileSize, 1f);
    }
    */

}
