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
        attackCooldown = playerStats.CurrentAttackSpeed;
        Debug.LogWarning(attackCooldown);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            firePoint.position = player.transform.position;
            FireProjectile();
            lastAttackTime = Time.time;
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // Ensure the z position is zero for 2D space
            Vector2 direction = (mouseWorldPosition - firePoint.position).normalized;

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            projectile.GetComponent<PlayerProjectile>().InitializeProjectile(direction, playerStats.CurrentProjectileSpeed, playerStats.CurrentDamage, playerStats.CurrentProjectileType);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = direction * playerStats.CurrentProjectileSpeed;
            }
        }
        else
        {
            Debug.LogError("Projectile prefab or fire point is not set.");
        }
    }
}
