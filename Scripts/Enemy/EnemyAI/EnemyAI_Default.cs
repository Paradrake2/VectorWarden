using UnityEngine;

public class EnemyAI_Default : MonoBehaviour
{
    public Transform player;
    public EnemyStats stats;
    public float attackCooldown = 1f;
    private float lastAttackTime = -999f;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitbox") && Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Enemy triggered with: " + other.name);
            lastAttackTime = Time.time;
            if (stats != null)
            {
                other.GetComponent<PlayerHitbox>()?.HitByEnemy(stats.damage);
            }
            else
            {
                Debug.Log("No enemy stats attached to this enemy!");
            }
        }
    }
}
