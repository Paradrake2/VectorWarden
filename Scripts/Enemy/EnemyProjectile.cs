using System;
using UnityEngine;

public enum EnemyProjectileType
{
    Normal,
    Explosive,
    Homing,
    Static
}
public class EnemyProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float lifetime = 5f; // Time before the projectile is destroyed
    public float rotationOffset = 0f;

    [Header("Projectile Settings")]
    public EnemyProjectileType projectileType;
    public float damage = 5f;
    public float speed = 10f;
    public float explosionRadius = 0f; // For explosive projectiles
    public float homingStrength = 0f;
    public float homingRange = 0f;
    public string enemyName;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Initialize(float damage, float speed, float lifetime = 5f)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (projectileType == EnemyProjectileType.Homing)
        {
            // Homing logic, needs to be able to be dodged by the player
        }
        if (projectileType == EnemyProjectileType.Explosive)
        {
            // Explosive logic, records the player's position at time of firing and explodes when reaching it
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("PlayerHitbox"))
        {
            PlayerHitbox player = collider.GetComponent<PlayerHitbox>();
            if (player != null)
            {
                player.HitByEnemy(damage, enemyName);
            }
            if (projectileType != EnemyProjectileType.Static)
            {
                Destroy(gameObject);
            }
        }
    }

}
