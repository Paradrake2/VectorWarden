using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DropPool
{
    public ItemRarity rarity;
    public List<Items> items;
}
public enum EnemyType
{
    Normal,
    Elite,
    Boss
}
public class EnemyStats : MonoBehaviour
{
    public EnemyRarity rarity;
    public float maxHealth;
    public float currentHealth;
    public float damage;
    public float defense;
    public float knockback;
    public float knockbackResistance;
    public float movementSpeed;
    public bool knockedBack = false;
    public float knockedBackTime = 0.1f;
    public bool wasHitByPlayer = false;
    public float colliderHeight = 1f;
    public float colliderWidth = 1f;

    [Header("Drops")]
    public float minXP;
    public float maxXP;
    public float minGold;
    public float maxGold;
    public List<DropPool> dropPool = new();
    public PlayerStats playerStats;

    public string id;
    void Awake()
    {
        //playerStats = FindFirstObjectByType<PlayerStats>();

        //if (playerStats == null) {
        //    Debug.LogError("PlayerStats not found");
        //}
    }
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    public void ApplyKnockback(Vector2 direction, float force, float duration) {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            StopCoroutine(KnockbackRoutine(direction, force, duration));
            StartCoroutine(WasHit());
            StartCoroutine(KnockbackRoutine(direction, force, duration));
        }

        
    }

    public float getMovementSpeed() {
        return movementSpeed;
    }
    public IEnumerator WasHit() {
        if (knockedBack) yield break;
        knockedBack = true;
        float originalMoveSpeed = movementSpeed;
        movementSpeed = 0;
        yield return new WaitForSeconds(knockedBackTime);
        movementSpeed = originalMoveSpeed;
        knockedBack = false;
    }
    
    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration) {
        float timer = 0f;
        while (timer < duration) {
            transform.position += (Vector3)(direction.normalized * force * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    
    }

    public float getKnockbackResistance() {
        return knockbackResistance;
    }

    public float getHealth() {
        return currentHealth;
    }
    public float getDamage() {
        return damage;
    }

    public float getDefense() {
        return defense;
    }
    public string getId()
    {
        return id;
    }
    public void SetID(string generatedID)
    {
        id = generatedID;
    }

    public float getXP()
    {
        return UnityEngine.Random.Range(minXP, maxXP);
    }

    public float getGold() {
        return UnityEngine.Random.Range(minGold,maxGold);
    }
    public List<Items> getDrop() {
        // float roll = UnityEngine.Random.value + PlayerStats.Instance.GetStat(StatType.DropRate);
        List<Items> finalDrops = new();

        //float dropRate = Mathf.Clamp01(playerStats.CurrentDropRate);
        float dropRate = 0.5f; // Placeholder for drop rate, can be adjusted or fetched from player stats
        Dictionary<ItemRarity, float> baseChances = new() {
            {ItemRarity.Gauranteed,1f},
            {ItemRarity.Common, 0.8f},
            {ItemRarity.Uncommon,0.4f},
            {ItemRarity.Rare, 0.2f},
            {ItemRarity.Epic, 0.08f},
            {ItemRarity.Legendary, 0.01f},
            {ItemRarity.Mythical, 0.005f}
        };
        foreach(var rarity in baseChances.Keys) {
            float effectiveChance = baseChances[rarity] + (dropRate * baseChances[rarity] * 2f);
            effectiveChance = Mathf.Min(effectiveChance, baseChances[rarity] * 20f);
            if (UnityEngine.Random.value <= effectiveChance) {
                var pool = dropPool.Find(p => p.rarity == rarity);
                if (pool != null && pool.items.Count > 0) {
                    finalDrops.Add(pool.items[UnityEngine.Random.Range(0, pool.items.Count)]);
                }
            }
        }
        return finalDrops;
    }
}
