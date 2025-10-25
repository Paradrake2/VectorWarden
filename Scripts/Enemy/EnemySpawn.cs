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
/*
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
*/
public class EnemySpawn : MonoBehaviour
{
    public static HashSet<string> enemyIDs = new HashSet<string>();

    public static EnemySpawn Instance;
    // Inspiration for this system came from https://www.youtube.com/watch?v=h2cg4ucDuWw and from Vampire Survivors itself
    [System.Serializable]
    public class EnemyWave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // List of enemy groups in this wave
        public int enemiesInWave;
        public float spawnInterval;
        public float spawnedEnemiesCount;
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; // Number of enemies to spawn in each wave
        public int spawnCount; // Number of enemies already spawned
        public GameObject enemyPrefab;
    }

    public List<EnemyWave> enemyWaves; // List of waves in game
    public int currentWaveIndex = 0; // Will go up when a boss is killed
    public int enemyStrengthIndex = 0; // Makes enemies stronger, increases every time a boss is killed
    public int enemiesAlive = 0; // How many enemies are currently alive
    public int maxEnemiesAllowed;
    public bool maxEnemiesReached;
    public List<GameObject> activeEnemies = new List<GameObject>();
    public float EnemyScaleFactor = 1.2f; // How much the enemies scale in strength. Eventually the player will be able to adjust this to increase the amount of rewards they gain
    public bool canSpawnEnemies = true;
    Transform player;

    [Header("Spawn Timing")]
    float spawnTimer;
    bool initialized = false;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //CalculateWaveQuota();
        player = FindFirstObjectByType<Player>().transform;
        //SpawnEnemies(); // Initial spawn of enemies
    }
    public void InitializeEnemyWaves(List<EnemyWave> waves)
    {
        enemyWaves = CloneWaves(waves);
        currentWaveIndex = 0;
        enemiesAlive = 0;
        maxEnemiesReached = false;
        spawnTimer = 0f;

        foreach (var w in enemyWaves)
        {
            w.spawnedEnemiesCount = 0;
            foreach (var g in w.enemyGroups)
            {
                g.spawnCount = 0;
            }
        }

        CalculateWaveQuota();
        initialized = true;
        if (player == null) player = FindFirstObjectByType<Player>().transform;
        SpawnEnemies();
    }
    List<EnemyWave> CloneWaves(List<EnemyWave> waves)
    {
        var clonedWaves = new List<EnemyWave>();
        foreach (var wave in waves)
        {
            var clonedWave = new EnemyWave
            {
                waveName = wave.waveName,
                enemyGroups = new List<EnemyGroup>(),
                enemiesInWave = wave.enemiesInWave,
                spawnInterval = wave.spawnInterval,
                spawnedEnemiesCount = wave.spawnedEnemiesCount
            };
            foreach (var group in wave.enemyGroups)
            {
                var clonedGroup = new EnemyGroup
                {
                    enemyName = group.enemyName,
                    enemyCount = group.enemyCount,
                    spawnCount = group.spawnCount,
                    enemyPrefab = group.enemyPrefab
                };
                clonedWave.enemyGroups.Add(clonedGroup);
            }
            clonedWaves.Add(clonedWave);
        }
        return clonedWaves;
    }
    public void SetEnemyWaves(List<EnemyWave> waves)
    {
        InitializeEnemyWaves(waves);
        Debug.Log("Enemy waves initialized with " + enemyWaves.Count + " waves.");
    }
    void Update()
    {
        if (!initialized || enemyWaves == null || enemyWaves.Count == 0) return;
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= enemyWaves[currentWaveIndex].spawnInterval)
        {
            spawnTimer = 0f;
            Debug.LogWarning("Spawning enemies for wave: " + enemyWaves[currentWaveIndex].waveName);
            SpawnEnemies();
        }
    }
    public void NextWave()
    {
        Debug.Log("Advancing to next wave");
        if (currentWaveIndex < enemyWaves.Count - 1)
        {
            currentWaveIndex++;
            CalculateWaveQuota();
            spawnTimer = 0f; // Reset spawn timer for the new wave
        }
        if (enemyStrengthIndex < 10) enemyStrengthIndex++;
        else { enemyStrengthIndex = Mathf.CeilToInt(enemyStrengthIndex * 1.2f); }
    }

    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in enemyWaves[currentWaveIndex].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        enemyWaves[currentWaveIndex].enemiesInWave = currentWaveQuota;
    }

    void SpawnEnemies()
    {
        if (maxEnemiesReached || !canSpawnEnemies) return;
    
        int capacity = maxEnemiesAllowed - enemiesAlive;
        if (capacity <= 0) { maxEnemiesReached = true; return; }// No capacity to spawn more enemies

        int spawnedThisTime = 0;
        var wave = enemyWaves[currentWaveIndex];

        foreach (var enemyGroup in wave.enemyGroups)
        {
            int remainingForGroup = Mathf.Max(0, enemyGroup.enemyCount - enemyGroup.spawnCount);
            int toSpawn = Mathf.Min(remainingForGroup, capacity - spawnedThisTime);

            for (int i = 0; i < toSpawn; i++)
            {
                GameObject enemy = Instantiate(enemyGroup.enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
                AdjustEnemy(enemy);
                enemyGroup.spawnCount++;
                enemyWaves[currentWaveIndex].spawnedEnemiesCount++;
                enemiesAlive++;
                spawnedThisTime++;
                if (spawnedThisTime >= capacity)
                {
                    break; // Stop spawning if we reached the capacity
                }
            }
        }

        bool waveComplete = true;
        foreach (var enemyGroup in wave.enemyGroups)
        {
            if (enemyGroup.spawnCount < enemyGroup.enemyCount)
            {
                waveComplete = false;
                break;
            }
        }

        // if all enemy groups hit their count, reset counters so wave can spawn again
        if (waveComplete)
        {
            foreach (var g in wave.enemyGroups) g.spawnCount = 0; // Reset spawn count for the next wave
            wave.spawnedEnemiesCount = 0;
        }

        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
    public void EnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0; // Prevent negative count
    }
    void AdjustEnemy(GameObject enemy)
    {
        var enemyStats = enemy.GetComponent<EnemyStats>();
        enemyStats.SetID(GenerateID()); // Assign a unique ID to enemy, needs to be cleared on death
        enemyStats.SetElite(ShouldSpawnElite(currentWaveIndex + 1));
        enemyStats.AdjustStats(enemyStrengthIndex);
    }
    Vector3 GetRandomSpawnPosition()
    {
        Vector3 playerPosition = player.position;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f).normalized;

        float distance = Random.Range(20f, 30f);
        return playerPosition + direction * distance;
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

    public void KillAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            Destroy(enemy);
        }
        activeEnemies.Clear();
    }
}

    
    // Below is the previous enemy spawning system, before it was revamped. It's kept here for reference
    /*
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
    */
