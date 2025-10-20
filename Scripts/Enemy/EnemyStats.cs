using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossDrops
{
    public List<Items> item;
    public int amount;
}


[System.Serializable]
public class DropPool
{
    public ItemRarity rarity;
    public List<GameObject> items;
}
public enum EnemyType
{
    Normal,
    Healer,
    Buffer,
    Boss
}
public class EnemyStats : MonoBehaviour
{
    public EnemyRarity rarity;
    public EnemyType type = EnemyType.Normal;
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

    [Header("Elite Stats")]
    public bool isElite = false;
    public float healthMultiplier = 2f;
    public float damageMultiplier = 1.5f;
    public float xpMultiplier = 4f;
    public float movementSpeedMultiplier = 1.2f;

    [Header("Drops")]
    public float minXP;
    public float maxXP;
    public float minGold;
    public float maxGold;
    public List<DropPool> dropPool = new();
    public List<DropPool> eliteDropPool = new();
    public PlayerStats playerStats;

    [Header("Healing")]
    public float healRadius = 5f;
    public float healAmount = 10f;
    public float healCooldown = 2.5f;

    [Header("Buffer")]
    public float bufferRadius = 5f;
    public float hpMult = 1.5f;
    public float damageMult = 1.5f;
    public float defenseMult = 1.5f;
    public bool bossSpawned = false;
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

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            StopCoroutine(KnockbackRoutine(direction, force, duration));
            StartCoroutine(WasHit());
            StartCoroutine(KnockbackRoutine(direction, force, duration));
        }


    }

    public float getMovementSpeed()
    {
        return movementSpeed;
    }
    public IEnumerator WasHit()
    {
        if (knockedBack) yield break;
        knockedBack = true;
        float originalMoveSpeed = movementSpeed;
        movementSpeed = 0;
        yield return new WaitForSeconds(knockedBackTime);
        movementSpeed = originalMoveSpeed;
        knockedBack = false;
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            transform.position += (Vector3)(direction.normalized * force * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

    }

    public float getKnockbackResistance()
    {
        return knockbackResistance;
    }

    public float getHealth()
    {
        return currentHealth;
    }
    public float getDamage()
    {
        return damage;
    }

    public float getDefense()
    {
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

    public float getGold()
    {
        return UnityEngine.Random.Range(minGold, maxGold);
    }
    public List<GameObject> getDrop()
    {
        if (bossSpawned) return null;
        // float roll = UnityEngine.Random.value + PlayerStats.Instance.GetStat(StatType.DropRate);
        List<GameObject> finalDrops = new();

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
        foreach (var rarity in baseChances.Keys)
        {
            float effectiveChance = baseChances[rarity] + (dropRate * baseChances[rarity] * 2f);
            effectiveChance = Mathf.Min(effectiveChance, baseChances[rarity] * 20f);
            if (UnityEngine.Random.value <= effectiveChance)
            {
                var pool = dropPool.Find(p => p.rarity == rarity);
                if (pool != null && pool.items.Count > 0)
                {
                    finalDrops.Add(pool.items[UnityEngine.Random.Range(0, pool.items.Count)]);
                }
            }
        }
        return finalDrops;
    }
    public void SetElite(bool elite)
    {
        isElite = elite;
        if (isElite)
        {
            ChangeStatsForElite();
            AddAuraEffect();
            Debug.Log("Elite enemy spawned with ID: " + id);
        }
        else
        {
            currentHealth = maxHealth;
        }
    }
    private void ChangeStatsForElite()
    {
        if (isElite)
        {
            maxHealth *= healthMultiplier;
            currentHealth = maxHealth;
            damage *= damageMultiplier;
            dropPool = eliteDropPool;
            minXP *= xpMultiplier;
            maxXP *= xpMultiplier;
            movementSpeed *= movementSpeedMultiplier;
        }
        else
        {
            currentHealth = maxHealth;
        }
    }
    public void AdjustStats(int strengthIndex)
    {
        if (strengthIndex <= 0) return;

        float baseGrowth = EnemySpawn.Instance.EnemyScaleFactor;
        float scale = Mathf.Pow(baseGrowth, strengthIndex - 1);
        currentHealth = Mathf.Floor(currentHealth * scale);
        maxHealth = Mathf.Floor(maxHealth * scale);
        damage = Mathf.Floor(damage * scale);
        defense = Mathf.Floor(defense * scale);
    }
    private void AddAuraEffect()
    {
        GameObject aura = new GameObject("Aura");
        aura.transform.SetParent(transform);
        aura.transform.localPosition = Vector3.zero;

        SpriteRenderer auraRenderer = aura.AddComponent<SpriteRenderer>();
        SpriteRenderer mainRenderer = GetComponent<SpriteRenderer>();

        if (mainRenderer != null)
        {
            auraRenderer.sprite = mainRenderer.sprite; // Use the same sprite
            auraRenderer.color = new Color(0f, 178f, 238f, 0.5f); // Red aura with transparency
            auraRenderer.sortingOrder = mainRenderer.sortingOrder - 1; // Render behind the main sprite
            auraRenderer.sortingLayerName = mainRenderer.sortingLayerName; // Match sorting layer
            aura.transform.localScale = Vector3.one * 1.3f; // Slightly larger than the main sprite
        }
    }
    public void FloorMult(int floor)
    {
        if (floor <= 1) return;

        float baseGrowth = 1.2f;
        float scale = Mathf.Pow(baseGrowth, floor - 1);
        maxHealth *= Mathf.Floor(maxHealth * scale);
        currentHealth = Mathf.Floor(currentHealth * scale);
        maxHealth = Mathf.Floor(maxHealth * scale);
        damage = Mathf.Floor(damage * scale);
        defense = Mathf.Floor(defense * scale);
    }

    public void TakeDamage(float amount)
    {
        float finalDamage = Mathf.Max(1, amount - defense);
        currentHealth -= finalDamage;
        if (currentHealth <= 0)
        {
            if (type == EnemyType.Normal)
            {
                GetComponent<Enemy>().Die();
            }
            if (type == EnemyType.Boss)
            {
                GetComponent<Boss>().Die();
            }
        }
    }
    public void Die()
    {
        if (type == EnemyType.Normal)
        {
            GetComponent<Enemy>().Die();
        }
        else if (type == EnemyType.Boss)
        {
            GetComponent<Boss>().Die();
        }
        else
        {
            Debug.LogWarning("Unknown enemy type on death.");
        }
        EnemySpawn.Instance.activeEnemies.Remove(gameObject);
    }
    public void AddHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Vector3 popupPosition = transform.position + Vector3.up * 1.2f;
        DamagePopup.Spawn(amount, popupPosition, Color.green);
    }
}
