using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject[] projectilePrefab;
    public PlayerStats playerStats;
    public Transform firePoint;
    public GameObject player;

    public float attackCooldown;
    [SerializeField] private float lastAttackTime = -999f; // serialize field to keep track of the last attack time
    private bool isFiring = false; // Track if the player is holding the fire button

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
    }

    void Update()
    {
        var activeProjectiles = playerStats.GetActiveProjectiles();
        if (activeProjectiles.Count > 0)
        {
            projectilePrefab[0] = activeProjectiles[0]; // Assumes only one projectile 
        }
        else if (projectilePrefab == null)
        {
            Debug.LogError("No active projectiles found. Please assign a projectile prefab.");
            return;
        }
        attackCooldown = playerStats.CurrentAttackCooldown * Mathf.Max(0.000001f, (1 - projectilePrefab[0].GetComponent<PlayerProjectile>().projectileData.attackSpeedModifier));
        if (playerStats.GetAutofireEnabled())
        {
            if (Input.GetMouseButtonDown(0) && !isFiring)
            {
                isFiring = true;
                StartCoroutine(FireContinuously());
            }

            if (Input.GetMouseButtonUp(0))
            {
                isFiring = false;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(attackCooldown);
                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    StartCoroutine(FireProjectile());
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    IEnumerator FireContinuously()
    {
        while (isFiring)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                firePoint.position = player.transform.position;
                StartCoroutine(FireProjectile());
                lastAttackTime = Time.time;
            }
            yield return null; // Wait for the next frame
        }
    }

    IEnumerator FireProjectile()
    {
        var projectilePrefabs = playerStats.GetActiveProjectiles();
        if (projectilePrefabs.Count == 0 && projectilePrefab != null)
        {
            projectilePrefabs.Add(projectilePrefab[0]);
        }

        int projectileCount = playerStats.GetProjectileCount();
        float spacing = 0.5f; // might be replaced with dynamic spacing logic

        foreach (var prefab in projectilePrefabs)
        {
            if (prefab != null && firePoint != null)
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0;
                Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;
                for (int i = 0; i < projectileCount; i++)
                {
                    float offset = (i - (projectileCount - 1) / 2f) * spacing;
                    Vector3 spawnPosition = firePoint.position + new Vector3(-direction.y, direction.x, 0) * offset;

                    GameObject projectile = Instantiate(prefab, spawnPosition, Quaternion.identity);

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
            }
            yield return new WaitForSeconds(0.05f); // delay between each projectile (adjust as needed)
        }
    }
}
