using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public EnemyStats enemyStats;
    public Boss boss;

    public BossAttackComponent spawnerAttackComponent;
    public BossAttackComponent fireProjectileComponent;
    public float attackInterval = 10f;
    public int enemySpawnAmount = 10;
    public int spawnAttackInterval = 3;
    private float lastSpawnTime;
    private int attackCounter = 0;
    void Start()
    {
        lastSpawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawnTime >= attackInterval)
        {
            lastSpawnTime = Time.time;
            Attack();
        }
    }
    void Attack()
    {
        if (attackCounter % spawnAttackInterval == 0) StartCoroutine(spawnerAttackComponent.ExecuteAttack(null));
        
        StartCoroutine(fireProjectileComponent.ExecuteAttack(null));
        attackCounter++;
    }

}
