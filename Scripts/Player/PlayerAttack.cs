using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public PlayerStats playerStats;
    public Transform firePoint;
    public GameObject player;

    public float attackCooldown;
    [SerializeField] private float lastAttackTime = -999f; // serialize field to keep track of the last attack time
    //public float rotationSpeed = 5f; // Optional: Speed of rotation if needed
    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats not found in the scene.");
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject not found in the scene.");
        }
        attackCooldown = playerStats.CurrentAttackCooldown;
        Debug.LogWarning(attackCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        ProjectileData data = projectilePrefab.GetComponent<PlayerProjectile>()?.projectileData;
        float attackSpeedModifier = data.attackSpeedModifier;
        float effectiveCooldown = attackCooldown *(1f - attackSpeedModifier);
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + effectiveCooldown)
        {
            Debug.Log("Attack cooldown: " + effectiveCooldown + " attackSpeedModifier: " + attackSpeedModifier);
            firePoint.position = player.transform.position;
            StartCoroutine(FireProjectile());
            lastAttackTime = Time.time;
        }
    }

    IEnumerator FireProjectile()
    {
        var projectilePrefabs = playerStats.GetActiveProjectiles();
        if (projectilePrefabs.Count == 0 && projectilePrefab != null)
        {
            projectilePrefabs.Add(projectilePrefab);
        }

        foreach (var prefab in projectilePrefabs)
        {
            if (prefab != null && firePoint != null)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0;
                Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;

                GameObject projectile = Instantiate(prefab, firePoint.position, Quaternion.identity);

                // Set rotation based on direction and spriteRotationOffset
                PlayerProjectile projScript = projectile.GetComponent<PlayerProjectile>();
                if (projScript != null)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    projectile.transform.rotation = Quaternion.Euler(0, 0, angle + projScript.spriteRotationOffset);

                    // Optionally, use InitializeProjectile for consistency
                    projScript.InitializeProjectile(direction, playerStats.CurrentProjectileSpeed, playerStats.CurrentDamage, projScript.projectileType);
                }
                else
                {
                    Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = direction * playerStats.CurrentProjectileSpeed;
                    }
                }
            }
            yield return new WaitForSeconds(0.05f); // delay between each projectile (adjust as needed)
        }
    }
}
