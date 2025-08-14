using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Boss : MonoBehaviour
{
    public EnemyStats stats;
    public PlayerStats playerStats;
    public BaseMovement movement;
    public List<BossDrops> bossDrops = new();
    public Image healthBar;
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        UIManager.Instance.ShowBossHealthbar();
        healthBar = GameObject.FindGameObjectWithTag("BossHealthBar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.UpdateMovement();
        healthBar.fillAmount = stats.currentHealth / stats.maxHealth;
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
        UIManager.Instance.HideBossHealthbar();
        EnemySpawn.Instance.NextWave();
        // Open path to the next floor
    }
}
