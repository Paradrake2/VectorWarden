using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerBasic : MonoBehaviour
{
    public EnemyStats stats;
    public Boss boss;
    public BossAIComponents bossAIComponents;
    public List<EnemyProjectile> projectileListOne;
    public List<EnemyProjectile> projectileListTwo;
    public GameObject previewPrefab;

    public Player player;

    public float health = 999999f; // placeholder for health, will be set in stats

    public float attackRange = 35f;
    public float attackCooldown = 2f;
    private float lastAttackTime;
    private int attackCounter = 0;
    public float projectileSpeed = 10f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        lastAttackTime = -attackCooldown; // Allow immediate attack on start
        health = stats.maxHealth; // Set health from stats
    }

    bool IsPlayerInRange()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer <= attackRange;
        }
        return false;
    }
    void Update()
    {
        if (IsPlayerInRange())
        {
            // Rotate to face the player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            // Attack the player
            AttackPlayer();
        }
        else
        {
            // Move towards the player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.position += direction * stats.movementSpeed * Time.deltaTime;
            // add function to destroy obstacles in the way
        }
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (attackCounter >= 5)
            {
                StartCoroutine(RapidFireAttack());
                StartCoroutine(bossAIComponents.PerformGridAttack(transform.position, 70f, 70f, 3f, projectileListTwo[Random.Range(0, projectileListTwo.Count)], previewPrefab));
                Debug.LogWarning("Rapid Fire Attack Triggered!");
                attackCounter = 0; // Reset counter after rapid fire
            }
            // Find a random projectile to use
            EnemyProjectile projectile = projectileListOne[Random.Range(0, projectileListOne.Count)];

            bossAIComponents.FireProjectile(projectile, transform.position, player.transform.position, player.GetComponent<PlayerMovement>().SmoothedVelocity);
            lastAttackTime = Time.time;
            attackCounter++;
        }
    }
    IEnumerator RapidFireAttack()
    {
        Debug.Log("Rapid Fire Attack!");
        EnemyProjectile projectile = projectileListOne[Random.Range(0, projectileListOne.Count)];

        // Stop moving while attacking
        int attackCount = Random.Range(10, 15); // Random number of attacks
        for (int i = 0; i < attackCount; i++)
        {
            Debug.LogWarning("Rapid Fire Attack: " + i);
            // Find a random projectile to use
            bossAIComponents.FireProjectile(projectile, transform.position, player.transform.position, player.GetComponent<PlayerMovement>().SmoothedVelocity);
            yield return new WaitForSeconds(0.16f);
        }
    }
    /*
    void FireProjectile()
    {
        GameObject newProj = Instantiate(projectile.gameObject, transform.position, Quaternion.identity);
        EnemyProjectile projData = newProj.GetComponent<EnemyProjectile>();
        projData.speed = projectileSpeed;
        projData.damage = stats.damage;

        float timeToIntercept = GetPredictedPosition(transform.position, player.transform.position, player.GetComponent<Rigidbody2D>().linearVelocity, projData.speed);
        if (timeToIntercept > 0)
        {
            Vector3 targetPosition = player.transform.position + (Vector3)(player.GetComponent<PlayerMovement>().SmoothedVelocity * timeToIntercept);
            Vector3 direction = (targetPosition - transform.position).normalized;
            newProj.GetComponent<Rigidbody2D>().linearVelocity = direction * projData.speed;
            newProj.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
        else
        {
            Debug.LogWarning("No valid intercept time found, firing directly at player.");
            Vector3 direction = (player.transform.position - transform.position).normalized;
            newProj.GetComponent<Rigidbody2D>().linearVelocity = direction * projData.speed;
            newProj.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }


        Vector3 toPlayer = (player.transform.position - transform.position).normalized;
Vector3 aimDir = direction; // The one from GetPredictedPosition

Debug.DrawRay(transform.position, aimDir * 3f, Color.blue, 2f);
Debug.DrawRay(transform.position, toPlayer * 3f, Color.green, 2f);

float dot = Vector3.Dot(toPlayer, aimDir);
        Debug.Log("DOT: " + dot);

    }
        */
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                player.TakeDamage(999999999, "Destroyer"); // Should instantly kill the player if they touch the boss
            }
        }
    }

    private float GetPredictedPosition(Vector2 shooterPos, Vector2 targetPos, Vector2 targetVelocity, float projectileSpeed)
    {
        Vector2 displacement = targetPos - shooterPos;
        float a = targetVelocity.sqrMagnitude - projectileSpeed * projectileSpeed;
        float b = 2f * Vector2.Dot(displacement, targetVelocity);
        float c = displacement.sqrMagnitude;

        float disc = b * b - (4f * a * c);
        if (disc < 0) return -1f; // No valid solution

        float sqrtDisc = Mathf.Sqrt(disc);
        float t1 = (-b+sqrtDisc) / (2f * a);
        float t2 = (-b-sqrtDisc) / (2f * a);

        float t = Mathf.Min(t1, t2);
        if (t < 0f) t = Mathf.Max(t1, t2);
        return t > 0f ? t : -1f; // Return the positive time or -1 if no valid time
    }

}
