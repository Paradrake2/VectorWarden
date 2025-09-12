using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public EnemyStats enemyStats;
    public Boss boss;
    public BossAIComponents bossAIComponents;

    public List<GameObject> enemyPrefabs1;
    public float spawnInterval = 5f;
    public int enemySpawnAmount = 10;
    private float lastSpawnTime;
    private List<Vector3> spawnPos;

    void Start()
    {
        lastSpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawnTime >= spawnInterval)
        {
            lastSpawnTime = Time.time;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        StartCoroutine(bossAIComponents.SummonEnemies(enemyPrefabs1, enemySpawnAmount, spawnPos,0));
    }

}
