using System;
using System.Collections;
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
    public bool knockedBack = false;
    public Rigidbody2D rb;
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!knockedBack) movement.UpdateMovement();

    }
    public void ApplyKnockback(Vector2 direction, float strength)
    {
        knockedBack = true;
        rb.AddForce(direction.normalized * strength, ForceMode2D.Impulse);
        StartCoroutine(ResetKnockback());
    }

    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.1f);
        rb.linearVelocity = Vector3.zero;
        knockedBack = false;
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
        List<GameObject> drops = stats.getDrop();
        foreach (var item in drops)
        {
            Instantiate(item, transform.position, Quaternion.identity);
        }
        playerStats.GainXP(UnityEngine.Random.Range(stats.minXP, stats.maxXP));
        playerStats.GainGold(Mathf.CeilToInt(UnityEngine.Random.Range(stats.minGold, stats.maxGold)));
        EnemySpawn.Instance.EnemyDied();
        Destroy(gameObject);
        FindFirstObjectByType<DungeonManager>().EnemyKilled();
    }


}
