using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "TrailAttack", menuName = "BossComponents/BossAttackComponents/TrailAttack")]
public class TrailAttack : BossAttackComponent
{
    public float damage;
    public float interval = 0.2f; // time between spawns of projectile
    public float duration = 1.5f; // time before projectile disappears
    public int amount; // amount of projectiles
    public EnemyProjectile projectilePrefab;

    void Setup()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        if (boss == null)
        {
            boss = FindFirstObjectByType<Boss>();
        }
    }
    public override IEnumerator ExecuteAttack(BossProjectileFactory bp)
    {
        Setup();
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = player.transform.position;
            yield return new WaitForSeconds(interval);
            GameObject proj = Instantiate(projectilePrefab.projectilePrefab, spawnPos, Quaternion.identity);
            proj.GetComponent<EnemyProjectile>().Initialize(damage, 0, duration);
            Destroy(proj, duration);
        }
    }
}
