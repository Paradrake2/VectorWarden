using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridAttack", menuName = "BossComponents/BossAttackComponents/GridAttack")]
public class GridAttack : BossAttackComponent
{
    public EnemyProjectile projectilePrefab;
    public GameObject previewPrefab;
    public float width;
    public float height;
    public float spacing = 1f;
    public Vector3 center;
    public float projectileLifetime = 2.4f;
    public float previewDuration = 1.0f;
    public void SetVar(EnemyProjectile proj, float w, float h, float sp, Vector3 cen, GameObject pr)
    {
        projectilePrefab = proj;
        width = w;
        height = h;
        spacing = sp;
        center = cen;
        previewPrefab = pr;
    }

    public override IEnumerator ExecuteAttack(BossProjectileFactory bp = null)
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
        yield return new WaitForSeconds(previewDuration);

        // Phase 2: spawn the actual (static) projectiles at selected spots
        foreach (var spawnPos in chosenSpots)
        {
            GameObject projGO = Object.Instantiate(projectilePrefab.gameObject, spawnPos, Quaternion.identity);
            var projData = projGO.GetComponent<EnemyProjectile>();
            projData.speed = 0; // static object
            projData.damage = projectilePrefab.damage;

            var rb = projGO.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = Vector2.zero; // static
        }
    }
}
