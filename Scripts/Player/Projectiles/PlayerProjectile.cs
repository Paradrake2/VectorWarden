using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal.Internal;
public enum ProjectileType
{
    Normal,
    Explosive,
    Piercing,
    Homing,
    Orbital,
    Area,
    Expanding,
    None
}

public class PlayerProjectile : MonoBehaviour
{
    public PlayerStats playerStats;
    public ProjectileData projectileData; // Reference to the projectile data
    public float projectileSpeed;
    public float damage;
    public ProjectileType projectileType;
    public float spriteRotationOffset = 0f; // Offset to align the sprite correctly

    private Vector2 moveDirection;

    public int pierceAmount;
    public float explosionRadius;
    public float homingRange;
    public float projectileSize = 1f;
    public float damageMultiplier = 1f; // Multiplier for damage based on player stats
    public float lifetime = 2f;
    private float knockbackForce = 1f; // Default knockback force
    public bool shotgunStyle = false;
    private float expansionRate = 0;
    private Rigidbody2D rb;
    private Vector2 inverseDirection;
    [SerializeField] private float homingTimer = 0f;
    [SerializeField] private const float homingInterval = 0.05f; // How often to check for homing targets


    private OrbitalAttack _orbitalOwner;
    private bool _notifiedOwner;
    public UnityEvent OnBegin, OnDone;
    public void SetOrbitalOwner(OrbitalAttack owner)
    {
        _orbitalOwner = owner;
    }
    void Awake()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats not found in the scene.");
            }
        }
    }
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        if (projectileData != null && projectileData.isHoming)
        {
            homingTimer += Time.deltaTime;
            if (homingTimer >= homingInterval)
            {
                homingTimer = 0f;
                // Implement homing behavior here
                Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, homingRange);
                Transform closestTarget = null;
                float closestDistance = Mathf.Infinity;

                foreach (var hit in targets)
                {
                    float dist = Vector2.Distance(transform.position, hit.transform.position);
                    if (hit.CompareTag("Enemy") && dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestTarget = hit.transform;
                    }
                }
                if (closestTarget != null)
                {
                    // Move towards the closest target
                    Vector2 direction = (closestTarget.position - transform.position).normalized;
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = direction * projectileSpeed;
                        //Debug.Log("playerStats.CurrentProjectileSpeed)" + playerStats.CurrentProjectileSpeed);
                        //Debug.Log("projectileData.baseSpeed)" + projectileData.baseSpeed);
                        //Debug.Log("projectileData.speedMultiplier)" + projectileData.speedMultiplier);
                        //Debug.Log("Final speed: " + speed);
                    }

                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + spriteRotationOffset));
                }
            }
        }
    }

    public void InitializeProjectile(Vector2 direction, float speed, float damageAmount, ProjectileType type, ProjectileData data = null, PlayerStats playerStats = null)
    {
        moveDirection = direction.normalized;
        inverseDirection = -moveDirection;
        if (data != null)
        {
            projectileData = data;
            spriteRotationOffset = projectileData.spriteRotationOffset;
            projectileSpeed = data.baseSpeed * data.speedMultiplier + playerStats.GetProjectileSpeed();
        }
        else
        {
            projectileSpeed = speed;
        }
        damage = damageAmount;
        projectileType = data.projectileType;

        SetupStats(data, playerStats);
        /*
        if (projectileData.isHoming && homingRange > 0f)
        {
            CircleCollider2D homingCollider = gameObject.AddComponent<CircleCollider2D>();
            homingCollider.radius = homingRange;
            homingCollider.isTrigger = true; // Make it a trigger to detect targets without physical collision
        }
        */



        if (ProjectileLevelTracker.Instance.GetLevel(data) != 0) // Check projectile tier
        {
            Debug.LogWarning($"Projectile tier {ProjectileLevelTracker.Instance.GetLevel(data)} detected, applying upgrades.");
            ApplyUpgradedStats(data.projectileUpgrade, ProjectileLevelTracker.Instance.GetLevel(data));
        }
        gameObject.transform.localScale = new Vector3(projectileSize, projectileSize, 1f);
        rb = GetComponent<Rigidbody2D>();
        HandleType(projectileType, moveDirection);
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + spriteRotationOffset));
    }
    void HandleType(ProjectileType type, Vector2 moveDirection)
    {
        switch (type)
        {
            case ProjectileType.Normal:
                rb.linearVelocity = moveDirection * projectileSpeed; // Assuming the projectile moves in the direction it's facing
                break;
            case ProjectileType.Explosive:
                rb.linearVelocity = moveDirection * projectileSpeed; // Assuming the projectile moves in the direction it's facing
                break;
            case ProjectileType.Piercing:
                rb.linearVelocity = moveDirection * projectileSpeed; // Assuming the projectile moves in the direction it's facing
                break;
            case ProjectileType.Homing:
                rb.linearVelocity = moveDirection * projectileSpeed; // Assuming the projectile moves in the direction it's facing
                break;
            case ProjectileType.Orbital:
                // Orbital projectile behavior
                break;
            case ProjectileType.Area:
                break;
            case ProjectileType.Expanding:
                rb.linearVelocity = moveDirection * projectileSpeed; // Assuming the projectile moves in the direction it's facing
                StartCoroutine(ExpandOverTime(expansionRate));
                break;
            default:
                Debug.LogWarning("Unhandled projectile type: " + type);
                break;
        }
    }
    void SetupStats(ProjectileData projectileData, PlayerStats playerStats)
    {
        if (playerStats != null)
        {
            pierceAmount = playerStats.GetPiercingAmount() + (int)projectileData.pierceAmount;
            explosionRadius = playerStats.GetExplosionRadius() + projectileData.explosionRadius;
            homingRange = playerStats.GetHomingRange() + projectileData.homingRange;
            projectileSize = playerStats.GetProjectileSize() + projectileData.projectileSize;
            expansionRate = projectileData.float1 + playerStats.GetExpansionSpeed();
        }
        else
        {
            pierceAmount = projectileData != null ? (int)projectileData.pierceAmount : 0;
            explosionRadius = projectileData != null ? projectileData.explosionRadius : 0f;
            homingRange = projectileData != null ? projectileData.homingRange : 0f;
            projectileSize = projectileData != null ? projectileData.projectileSize : 1f;
        }
    }
    void ApplyUpgradedStats(ProjectileUpgrade upgrade, int tier)
    {
        if (upgrade == null) return;

        projectileSpeed = projectileSpeed * upgrade.tiers[tier].speedMultiplier;
        damage = damage * upgrade.tiers[tier].damageMultiplier + upgrade.tiers[tier].flatDamage;
        Debug.Log(damage);
        projectileSize = projectileSize * upgrade.tiers[tier].sizeMult;
        pierceAmount = pierceAmount + upgrade.tiers[tier].piercingAddition;
        explosionRadius = explosionRadius + upgrade.tiers[tier].explosionRadius;
        knockbackForce = knockbackForce + upgrade.tiers[tier].knockbackAddition;
        homingRange = homingRange + upgrade.tiers[tier].homingRange;
        shotgunStyle = upgrade.tiers[tier].shotgunStyle;
        if (upgrade.tiers[tier].unlockHoming)
        {
            projectileData.isHoming = true;
        }
        if (upgrade.tiers[tier].unlockExplosive)
        {
            projectileData.isExplosive = true;
        }
        if (projectileType == ProjectileType.Expanding)
        {
            // Use float1 as expansion rate
            expansionRate = expansionRate + upgrade.tiers[tier].float1;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = collision.gameObject.GetComponent<EnemyStats>();
            collision.GetComponent<Enemy>()?.ApplyKnockback(-inverseDirection, knockbackForce * (1 - enemyStats.getKnockbackResistance()));
            if (enemyStats != null && explosionRadius <= 0f)
            {
                CalcEnemyDamage(damage, enemyStats);
                if (enemyStats.currentHealth <= 0)
                {
                    ColliderDie(collision);
                }
            }
            pierceAmount--;
            if (playerStats.HasSpecialEffect(SpecialEffectType.Lifesteal))
            {
                Debug.Log("Lifesteal applied");
                playerStats.GainHealth(damage * playerStats.GetLifestealAmount());
            }
            if (playerStats.GetExplosionRadius() + explosionRadius > 0f && projectileType == ProjectileType.Explosive)
            {
                Explode();
                Vector3 popupPosition = transform.position;
                ExplosionRadiusManager.Instance.SpawnRadiusIndicator(popupPosition, explosionRadius);
            }
            if (pierceAmount < 0) DestroyGameobject();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            DestroyGameobject();
        }
    }
    void ColliderDie(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                if (collision.GetComponent<Enemy>() != null)
                {
                    collision.GetComponent<Enemy>().Die();
                }
                else if (collision.GetComponent<Boss>() != null)
                {
                    collision.GetComponent<Boss>().Die();
                }
            }
        }
    }
    void DestroyGameobject()
    {
        if (projectileType == ProjectileType.Orbital)
        {
            NotifyOwnerOnce();
        }
        Destroy(gameObject);
    }
    private void NotifyOwnerOnce()
    {
        if (_notifiedOwner) return;
        _notifiedOwner = true;
        _orbitalOwner?.OnOrbitalExpired(this);
    }

    void CalcEnemyDamage(float dmgAmount, EnemyStats stats)
    {
        float defense = stats.defense;
        float finalDamage = Mathf.Max(1, dmgAmount - defense);
        Vector3 popupPosition = stats.transform.position + Vector3.up * 1.2f;
        DamagePopup.Spawn(finalDamage, popupPosition, Color.red);
        stats.currentHealth -= finalDamage;
    }

    void Explode()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        // Graphics, scale is proportional to explosionRadius
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                if (enemyStats != null)
                {
                    CalcEnemyDamage(damage, enemyStats);
                    Vector3 popupPosition = enemy.transform.position + Vector3.up * 1.2f;
                    DamagePopup.Spawn(damage, popupPosition, Color.red);
                    //                    Debug.Log("BOOM");
                    if (enemyStats.currentHealth <= 0)
                    {
                        enemyStats.Die();
                    }
                }
            }
        }
        Destroy(gameObject);
    }

    public void SetHomingTarget(Transform target)
    {
        if (projectileData != null && projectileData.isHoming)
        {
            // Implement homing logic here
            Vector2 direction = (target.position - transform.position).normalized;
            moveDirection = direction;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = moveDirection * projectileSpeed;
                Debug.Log($"Homing towards target: {target.name} with speed: {projectileSpeed}");
            }
        }
    }

    IEnumerator ExpandOverTime(float expansionRate)
    {
        while (true)
        {
            projectileSize += expansionRate * Time.deltaTime;
            transform.localScale = new Vector3(projectileSize, projectileSize, 1f);
            yield return null;
        }
    }

    
}
