using UnityEngine;

public class EnemyAI_BasicRanged : MonoBehaviour
{
    public GameObject projectile;
    public EnemyStats stats;
    public float projectileSpeed = 10f; // Speed of the projectile
    public float attackRange = 10f; // Range at which the enemy can attack
    public float attackCooldown = 2f; // Time between attacks
    private float lastAttackTime;


    void Start()
    {
        lastAttackTime = Time.time;
    }
    void Update()
    {
        RotateToFacePlayer();
        AttackPlayer();
    }
    

    private bool IsPlayerInRange()
    {
        // Assuming player is a GameObject with a tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceToPlayer <= attackRange;
        }
        return false;
    }

    private void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            FireProjectile();
            // Fire projectile sound
            Debug.Log("Attacking Player!");
            lastAttackTime = Time.time;
        }
    }

    void FireProjectile()
    {
        if (projectile != null)
        {
            GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
            EnemyProjectile projData = newProjectile.GetComponent<EnemyProjectile>();
            projData.speed = projectileSpeed;
            projData.damage = stats.damage;
            // Set the projectile's direction towards the player or a specific target
            Vector3 direction = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized;
            newProjectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projectileSpeed; // Adjust speed as necessary
            newProjectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
            Destroy(newProjectile, projData.lifetime); // Destroy after a certain time to prevent memory
        }
    }
    private void RotateToFacePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
