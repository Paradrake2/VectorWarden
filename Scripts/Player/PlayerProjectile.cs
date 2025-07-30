using UnityEditor.Experimental.GraphView;
using UnityEngine;
public enum ProjectileType
{
    Normal,
    Explosive,
    Piercing,
    Homing,
    None
}
public class PlayerProjectile : MonoBehaviour
{
    public PlayerStats playerStats;
    public float projectileSpeed;
    public float damage;
    public ProjectileType projectileType;
    public float spriteRotationOffset = 0f; // Offset to align the sprite correctly

    private Vector2 moveDirection;
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
        projectileSpeed = playerStats.CurrentProjectileSpeed;
        damage = playerStats.CurrentDamage;
        projectileType = playerStats.CurrentProjectileType;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitializeProjectile(Vector2 direction, float speed, float damageAmount, ProjectileType type)
    {
        moveDirection = direction.normalized;
        projectileSpeed = speed;
        damage = damageAmount;
        projectileType = type;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * projectileSpeed; // Assuming the projectile moves in the direction it's facing
        }
        
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + spriteRotationOffset));
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = collision.gameObject.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.currentHealth -= damage;
                Debug.LogWarning($"Enemy hit! Remaining Health: {enemyStats.currentHealth}");
                Debug.LogWarning(collision.gameObject.name);
                if (enemyStats.currentHealth <= 0)
                {
                    collision.GetComponent<Enemy>().Die();
                }
            }
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
