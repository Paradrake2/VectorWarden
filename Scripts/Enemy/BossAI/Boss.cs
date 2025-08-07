using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public EnemyStats stats;
    public PlayerStats playerStats;
    public BaseMovement movement;
    public List<BossDrops> bossDrops = new();
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.UpdateMovement();
    }
    public void Die()
    {
        foreach (var drop in bossDrops)
        {
            foreach (var item in drop.item)
            {
                InventorySystem.Instance.AddItemToSpoils(item.itemName, drop.amount);
            }
        }
        playerStats.GainXP(Random.Range(stats.minXP, stats.maxXP));
        Destroy(gameObject);
        // Open path to the next floor
    }
}
