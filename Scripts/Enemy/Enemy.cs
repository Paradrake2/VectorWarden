using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public BaseMovement movement;
    public EnemyStats stats;
    public PlayerStats playerStats;
    public event Action OnDeath;
    public TextMeshProUGUI damageIndicator;
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        movement.UpdateMovement();
    }
    public void TakeDamage(float damage)
    {
        if (stats == null)
        {
            Debug.LogWarning("Enemy stats not set.");
            return;
        }

        float finalDamage = Mathf.Max(0, damage - stats.defense);
        stats.currentHealth -= finalDamage;

        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        OnDeath?.Invoke();
        List<Items> drops = stats.getDrop();
        foreach (var item in drops)
        {
            InventorySystem.Instance.AddItemToSpoils(item.ID, 1);
        }
        playerStats.GainXP(UnityEngine.Random.Range(stats.minXP, stats.maxXP));
        playerStats.GainGold(Mathf.CeilToInt(UnityEngine.Random.Range(stats.minGold, stats.maxGold)));
        Destroy(gameObject);
    }

    void ShowDamage(float damage)
    {
        // Instantiate a damage indicator
    }

}
