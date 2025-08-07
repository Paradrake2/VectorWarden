using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FloormasterSpawner
{
    public int minFloor; // Minimum floor in the range
    public int maxFloor; // Maximum floor in the range
    public List<GameObject> floormasters;
}

public class Floormaster : MonoBehaviour
{
    public static Floormaster Instance;
    public DungeonManager dungeonManager;
    public List<FloormasterSpawner> floormasters;
    public void SpawnFloormaster()
    {
        int floor = DungeonManager.Instance.floor;
        FloormasterSpawner spawner = floormasters.FirstOrDefault(s => floor >= s.minFloor && floor <= s.maxFloor);
        if (spawner != null)
        {
            GameObject floormaster = spawner.floormasters[Random.Range(0, spawner.floormasters.Count)];
            Instantiate(floormaster, GetSpawnPosition(), Quaternion.identity);
            // Create arrow indicator pointing to floormaster
        }
        DungeonManager.Instance.floormasterSpawned = true;
    }
    Vector3 GetSpawnPosition()
    {
        Vector3 playerPosition = FindFirstObjectByType<PlayerStats>().transform.position;
        float angle = Random.Range(0f, Mathf.PI * 2); // get random direction on unit circle
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
        float distance = Random.Range(15f, 30f);
        return playerPosition + direction * distance;
    }
    void Start()
    {
        dungeonManager = FindFirstObjectByType<DungeonManager>();
        if (dungeonManager == null)
        {
            Debug.LogError("DungeonManager not found in the scene.");
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
