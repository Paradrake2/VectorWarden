using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnEnemyAttack", menuName = "BossComponents/BossAttackComponents/SpawnEnemyAttack")]
public class SpawnEnemyAttack : BossAttackComponent
{
    public List<GameObject> enemyPrefabs;
    public int spawnNum = 10;
    public float spawnInterval = 0.1f;
    public float spawnHeight = 10f;
    public float spawnWidth = 10f;
    public float spacing = 1f;
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
    List<Vector3> ChosenPos()
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 center = boss.transform.position;
        float halfW = spawnWidth * 0.5f;
        float halfH = spawnHeight * 0.5f;
        for (int i = 0; i < spawnNum; i++)
        {
            float x = Random.Range(center.x - halfW, center.x + halfW);
            float y = Random.Range(center.y - halfH, center.y + halfH);
            Vector3 pos = new Vector3(x, y, 0f);
            positions.Add(pos);
        }
        return positions;
    }
    public override IEnumerator ExecuteAttack(BossProjectileFactory bp)
    {
        Setup();
        List<Vector3> spawnPositions = ChosenPos();
        foreach (Vector3 pos in spawnPositions)
        {
            int index = Random.Range(0, enemyPrefabs.Count);
            GameObject enemyPrefab = enemyPrefabs[index];
            GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            // the enemy prefab already shouldn't have any loot attached to it, but just in case this makes sure no loot/xp is dropped when killed
            enemy.GetComponent<EnemyStats>().bossSpawned = true; // prevent this enemy from dropping loot
            yield return new WaitForSeconds(spawnInterval);
        }
    }

}
