using System.Collections;
using UnityEngine;

public class EnemyAI_Healer : MonoBehaviour
{
    public EnemyStats enemyStats;
    public GameObject healEffectPrefab;
    public float healPrefabLifetime = 0.5f;
    private float healTimer = 0;
    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    void Update()
    {
        if (healTimer <= 0)
        {
            HealEnemies();
            healTimer = enemyStats.healCooldown; // Reset the timer
        }
        else
        {
            healTimer -= Time.deltaTime;
        }
    }

    void HealEnemies()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, enemyStats.healRadius);
        StartCoroutine(HealEffect());
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyStats targetStats = hitCollider.GetComponent<EnemyStats>();
                if (targetStats != null && targetStats != enemyStats && targetStats.type != EnemyType.Healer)
                {
                    targetStats.AddHealth(enemyStats.healAmount);
                }
            }
        }
    }
    IEnumerator HealEffect()
    {
        GameObject healEffect = Instantiate(healEffectPrefab, transform.position, Quaternion.identity);
        healEffect.transform.localScale = new Vector3(enemyStats.healRadius, enemyStats.healRadius, 1);
        healEffect.GetComponent<EnemyProjectileDestroy>().lifetime = healPrefabLifetime; // This makes sure the heal prefab is destroyed even if the enemy is killed first
        yield return new WaitForSeconds(healPrefabLifetime);
        Destroy(healEffect);
    }
}
