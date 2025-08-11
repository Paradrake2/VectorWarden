using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum EnemyRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythical
}
[System.Serializable]
public class EnemyArea
{
    public string areaName;
    public List<EnemyRarityGroup> rarityGroups;
}
[System.Serializable]
public class EnemyRarityGroup
{
    public EnemyRarity rarity;
    public List<GameObject> enemies;
}
public class EnemySpawn : MonoBehaviour
{
    public static HashSet<string> enemyIDs = new HashSet<string>();
    public MapGenerator mapGenerator;
    private EnemyRarity ChooseRarity()
    {
        float roll = Random.value;
        if (roll <= 0.005f) return EnemyRarity.Mythical;
        else if (roll <= 0.05f) return EnemyRarity.Legendary;
        else if (roll <= 0.11f) return EnemyRarity.Epic;
        else if (roll <= 0.23f) return EnemyRarity.Rare;
        else if (roll <= 0.35f) return EnemyRarity.Uncommon;
        else return EnemyRarity.Common;
    }
    public List<EnemyArea> areaEnemyLists;
    private List<GameObject> activeEnemies = new List<GameObject>();

    public float spawnCheckInterval = 0.2f;
    private float lastSpawnCheckTime = 0f;
    public void SpawnEnemy(Vector3 position)
    {
        int areaIndex = Mathf.Clamp(GameManager.Instance.enemyLevel, 0, areaEnemyLists.Count - 1);
        EnemyArea area = areaEnemyLists[areaIndex];
        var rarityGroups = area.rarityGroups;
        EnemyRarity selectedRarity = ChooseRarity();
        var group = rarityGroups.FirstOrDefault(gameObject => gameObject.rarity == selectedRarity);
        if (group == null)
        {
            Debug.LogError("EnemyRarityGroup is null!");
            return;
        }
        if (group == null || group.enemies.Count == 0)
        {
            // Fall back to common enemies
            group = rarityGroups.FirstOrDefault(g => g.rarity == EnemyRarity.Common);
            if (group == null || group.enemies.Count == 0)
            {
                Debug.LogWarning("No valid enemies to spawn in area " + areaIndex);
                return;
            }
        }
        GameObject chosenEnemy = group.enemies[Random.Range(0, group.enemies.Count)];
        if (chosenEnemy == null)
        {
            chosenEnemy = group.enemies[0];
            Debug.LogWarning("was null");
        }
        bool isElite = ShouldSpawnElite(GameManager.Instance.enemyLevel);
        var enemy = Instantiate(chosenEnemy, position, Quaternion.identity);
        enemy.GetComponent<EnemyStats>().SetID(GenerateID());
        enemy.GetComponent<EnemyStats>().SetElite(isElite);
        enemy.GetComponent<EnemyStats>().FloorMult(GameManager.Instance.enemyLevel);
        activeEnemies.Add(enemy);
        enemy.GetComponent<Enemy>().OnDeath += () => activeEnemies.Remove(enemy); // Remove from list on death

    }
    string GenerateID()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int idLength = 10;
        string id;
        int maxAttempts = 100;

        do
        {
            id = new string(Enumerable.Repeat(chars, idLength).Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
            maxAttempts--;
        }
        while (IsIDTaken(id) && maxAttempts > 0);

        if (maxAttempts == 0)
        {
            Debug.LogError("Failed to generate unique Equipment ID after 100 attempts");
            return null;
        }

        RegisterID(id);
        return id;
    }

    public static bool IsIDTaken(string id)
    {
        return enemyIDs.Contains(id);
    }

    public static void RegisterID(string id)
    {
        enemyIDs.Add(id);
    }
    private bool ShouldSpawnElite(int floor)
    {
        float eliteChance = Mathf.Clamp(floor * 0.02f, 0, 0.5f); // Max 50% chance
        return Random.value < eliteChance;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpawnCheckTime >= spawnCheckInterval)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawnEnemies();
        }
    }

    private void TrySpawnEnemies()
    {
        if (activeEnemies.Count >= GameManager.Instance.maxEnemies) return;

        int spawnAttempts = 0;
        int maxSpawnAttempts = 100;

        while (activeEnemies.Count < GameManager.Instance.maxEnemies && spawnAttempts < maxSpawnAttempts)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            SpawnEnemy(spawnPosition);
            spawnAttempts++;
        }
        if (spawnAttempts >= maxSpawnAttempts)
        {
            Debug.LogWarning("Reached maximum spawn attempts. Check spawn logic.");
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 playerPosition = FindFirstObjectByType<Player>().transform.position;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f).normalized;

        float distance = Random.Range(20f, 30f);
        return playerPosition + direction * distance;
    }
}
