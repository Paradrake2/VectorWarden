using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackPattern
{
    IEnumerator ExecuteAttack();
}

public interface IAttackContext
{
    Transform Boss { get; }
    Transform Player { get; }
    BossProjectileFactory Projectiles { get; }
    float DeltaTime { get; }

}
public class BossAIComponents : MonoBehaviour
{
    public static BossAIComponents Instance;
    void Awake()
    {
        Instance = this;
    }
    public bool IsPlayerInRange(Vector3 playerPosition, Vector3 bossPosition, float range)
    {
        return Vector3.Distance(playerPosition, bossPosition) <= range;
    }

    
    // Grid attack
    public IEnumerator PerformGridAttack(
    Vector3 center, float width, float height, float spacing,
    EnemyProjectile projectilePrefab, GameObject previewPrefab)
    {
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;
        Debug.LogWarning("Performing grid attack at " + center + " with width " + width + " and height " + height);
        // Compute integer counts to avoid float drift / double-inclusion on edges
        int cols = Mathf.FloorToInt(width / spacing) + 1;   // include both edges
        int rows = Mathf.FloorToInt(height / spacing) + 1;

        var chosenSpots = new List<Vector3>();

        // Phase 1: show previews and remember chosen spots (30% chance)
        for (int ix = 0; ix < cols; ix++)
        {
            float x = -halfW + ix * spacing;
            for (int iy = 0; iy < rows; iy++)
            {
                float y = -halfH + iy * spacing;
                if (Random.value <= 0.7f)
                {
                    Vector3 spawnPos = center + new Vector3(x, y, 0f);
                    var preview = Object.Instantiate(previewPrefab, spawnPos, Quaternion.identity);
                    Object.Destroy(preview, 1.0f);
                    chosenSpots.Add(spawnPos);
                }
            }
        }

        // Wait once, not per-cell
        yield return new WaitForSeconds(1f);

        // Phase 2: spawn the actual (static) projectiles at selected spots
        foreach (var spawnPos in chosenSpots)
        {
            GameObject projGO = Object.Instantiate(projectilePrefab.gameObject, spawnPos, Quaternion.identity);
            var projData = projGO.GetComponent<EnemyProjectile>();
            projData.speed = projectilePrefab.speed;
            projData.damage = projectilePrefab.damage;

            var rb = projGO.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = Vector2.zero; // static
        }
    }
    public IEnumerator TrailAttack(float amount, EnemyProjectile projectile, GameObject player)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = player.transform.position;
            yield return new WaitForSeconds(0.25f);
            GameObject proj = Instantiate(projectile.projectilePrefab, spawnPos, Quaternion.identity);
            Destroy(proj, 2f);
        }

    }

    public void ToggleEnemySummon(bool var)
    {
        EnemySpawn.Instance.canSpawnEnemies = var;
    }

    // Basic summon enemies function, randomly chooses enemies and their positions based off of a given list
    public IEnumerator SummonEnemies(List<GameObject> enemyPrefabs, int amount, List<Vector3> spawnPoints, float delay)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            Vector3 spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(delay);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
