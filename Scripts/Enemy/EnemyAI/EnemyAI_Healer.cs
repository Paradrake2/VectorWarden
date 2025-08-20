using System.Collections;
using UnityEngine;

public class EnemyAI_Healer : MonoBehaviour
{
    public EnemyStats enemyStats;
    public GameObject healEffectPrefab;
    private float healTimer = 0;
    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    // Update is called once per frame
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
        yield return new WaitForSeconds(0.5f);
        Destroy(healEffect);
    }
}
