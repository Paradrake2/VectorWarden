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
    public void SpawnEnemy(Vector3 position)
    {
        int areaIndex = DungeonManager.Instance.floor / 10;
        areaIndex = Mathf.Clamp(areaIndex, 0, areaEnemyLists.Count - 1);
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
        chosenEnemy.GetComponent<EnemyStats>().SetID(GenerateID());
        var enemy = Instantiate(chosenEnemy, position, Quaternion.identity);
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
