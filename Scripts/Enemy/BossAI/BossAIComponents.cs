using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAIComponents : MonoBehaviour
{
    public static BossAIComponents Instance;
    void Start()
    {
        Instance = this;
    }
    public bool IsPlayerInRange(Vector3 playerPosition, Vector3 bossPosition, float range)
    {
        return Vector3.Distance(playerPosition, bossPosition) <= range;
    }

    public void FireProjectile(EnemyProjectile projectilePrefab, Vector3 shooterPos, Vector3 targetPos, Vector2 targetVelocity)
    {
        GameObject projectile = Instantiate(projectilePrefab.gameObject, shooterPos, Quaternion.identity);
        EnemyProjectile projData = projectile.GetComponent<EnemyProjectile>();
        projData.speed = projectile.GetComponent<EnemyProjectile>().speed;
        projData.damage = projectile.GetComponent<EnemyProjectile>().damage;

        float timeToIntercept = GetPredictedPosition(shooterPos, targetPos, targetVelocity, projData.speed);
        if (timeToIntercept > 0)
        {
            Vector3 targetPosition = targetPos + (Vector3)(targetVelocity * timeToIntercept);
            Vector3 direction = (targetPosition - shooterPos).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projData.speed;
            projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
        else
        {
            Vector3 direction = (targetPos - shooterPos).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projData.speed;
            projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }
    public float GetPredictedPosition(Vector3 shooterPos, Vector3 targetPos, Vector2 targetVelocity, float projectileSpeed)
    {
        Vector2 displacement = targetPos - shooterPos;
        float a = targetVelocity.sqrMagnitude - projectileSpeed * projectileSpeed;
        float b = 2f * Vector2.Dot(displacement, targetVelocity);
        float c = displacement.sqrMagnitude;

        float disc = b * b - (4f * a * c);
        if (disc < 0) return -1f; // No valid solution

        float sqrtDisc = Mathf.Sqrt(disc);
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);

        float t = Mathf.Min(t1, t2);
        if (t < 0f) t = Mathf.Max(t1, t2);
        return t > 0f ? t : -1f; // Return the positive time or -1 if no valid time
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
