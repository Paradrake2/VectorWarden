using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MapType
{
    Arena,
    Dungeon,
    OpenWorld
}

public enum EnvironmentalObject
{
    Ore,
    Rock,
    Tree,
    Water,
    Wall
}
[System.Serializable]
public class EnvironmentalResourceData
{
   // public EnvironmentalObject type;
    public GameObject prefab;
    public float baseRarityWeight;
    public float weightPerFloor;
    public int floorUnlock; // the floor at which this resource becomes available
}
public class MapGenerator : MonoBehaviour
{
    public List<RoomTemplate> roomTemplates;
    public Tilemap tilemap;
    public TileBase[] floorPrefab;
    public GameObject[] wallPrefab;
    public GameObject[] obstaclePrefab;
    public GameObject[] envLootPrefab;
    public Vector3 playerSpawnPosition;
    public List<Vector3> enemySpawnPoints = new List<Vector3>();
    public EnvironmentalResourceData[] orePrefab;
    public PathfindingNode[,] grid;
    public int gridResolution = 2; // how many nodes per tile
    private bool[,] map;
    public EnemySpawn enemySpawn;
    public Vector2Int arenaOffset;

    private List<Vector3> pendingUnwalkables = new();
    public void GenerateRoom()
    {
        int currentFloor = DungeonManager.Instance.getFloor();
        var eligibleTemplates = roomTemplates.ToList();

        var template = eligibleTemplates[UnityEngine.Random.Range(0, eligibleTemplates.Count)];
        int width = UnityEngine.Random.Range(template.minWidth, template.maxWidth);
        int height = UnityEngine.Random.Range(template.minHeight, template.maxHeight);
        int radius = UnityEngine.Random.Range(template.minRadius, template.maxRadius);

        float enemyDensity = Random.Range(template.minEnemyDensity, template.maxEnemyDensity);
        float obstacleDensity = Random.Range(template.minObstacleWeight, template.maxObstacleWeight);
        float oreDensity = Random.Range(template.minOreDensity, template.maxOreDensity);
        float chestDensity = Random.Range(template.minChestDensity, template.maxChestDensity);

        RoomType roomType = template.roomType;
        map = new bool[width, height];

        GenerateArenaMap(radius, enemyDensity, obstacleDensity, oreDensity, chestDensity, roomType);

        playerSpawnPosition = GenerateRandomPosition();
        //OnDrawGizmos();
    }
    /*
    public void OnDrawGizmos()
    {
        if (grid == null)
            return;

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var node = grid[x, y];
                Gizmos.color = node.walkable ? Color.green : Color.red;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (1f / gridResolution) * 0.9f);
            }
        }
    }
*/
    void BuildNodeGrid()
    {
        int mapWidth = map.GetLength(0);
        int mapHeight = map.GetLength(1);
        grid = new PathfindingNode[mapWidth * gridResolution, mapHeight * gridResolution];

        for (int x = 0; x < mapWidth * gridResolution; x++)
        {
            for (int y = 0; y < mapHeight * gridResolution; y++)
            {
                int mapX = x / gridResolution;
                int mapY = y / gridResolution;

                bool walkable = map[mapX, mapY];

                // Convert grid coordinates to world space
                float worldX = (mapX + arenaOffset.x) + (0.5f / gridResolution);
                float worldY = (mapY + arenaOffset.y) + (0.5f / gridResolution);

                PathfindingNode node = new PathfindingNode(x, y, walkable);
                node.worldPosition = new Vector3(worldX, worldY, 0);
                //node.worldPosition = new Vector3(worldX, worldY, 0);
                grid[x, y] = node;
            }
        }
        foreach (var tilePos in pendingUnwalkables)
        {
            MakeUnwalkable(tilePos);
            //Debug.Log($"Made unwalkable: {tilePos}");
        }
        pendingUnwalkables.Clear();
    }

    // Arena generation
    void GenerateArenaMap(int radius, float enemyDensity, float obstacleDensity, float oreDensity, float chestDensity, RoomType roomType)
    {
        arenaOffset = new Vector2Int(-radius, -radius);
        Vector2 center = new Vector2(0, 0);
        int diameter = radius * 2 + 1;
        map = new bool[diameter, diameter];
        int offset = radius;
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                float distSq = Vector2.SqrMagnitude(gridPos - center);

                // Floor within circle
                if (distSq <= radius * radius)
                {
                    TileBase floor = floorPrefab[Random.Range(0, floorPrefab.Length)];
                    tilemap.SetTile(tilePos, floor);
                    map[x + offset, y + offset] = true;
                    GenerateContents(tilePos, enemyDensity, obstacleDensity, oreDensity, chestDensity, roomType);
                }

                // Walls around edge
                if (distSq >= (radius - 1) * (radius - 1) && distSq <= radius * radius)
                {
                    Vector3 spawnpos = tilemap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);
                    GameObject wall = Instantiate(wallPrefab[Random.Range(0, wallPrefab.Length)], spawnpos, Quaternion.identity);
                    wall.transform.parent = this.transform;

                }
            }
        }
        BuildNodeGrid();
        foreach (Vector3 pos in enemySpawnPoints)
        {
            Vector3Int cellPos = tilemap.WorldToCell(pos);
            TileBase tile = tilemap.GetTile(cellPos);

            if (tile != null && IsInsideFloorRadius(cellPos, radius)) enemySpawn.SpawnEnemy(pos);
        }
    }

    bool IsInsideFloorRadius(Vector3Int cellPos, int radius)
    {
        Vector2 center = Vector2.zero;
        float distSq = (new Vector2(cellPos.x, cellPos.y) - center).sqrMagnitude;
        return distSq <= (radius - 1) * (radius - 1);
    }







    // Content generation
    void GenerateContents(Vector3Int tilePos, float enemyDensity, float obstacleDensity, float oreDensity, float chestDensity, RoomType roomType)
    {
        Vector3 spawnPos = new Vector3(tilePos.x, tilePos.y, 0) + new Vector3(0.5f, 0.5f, 0);
        if (tilemap.HasTile(tilePos) && Random.value < enemyDensity) enemySpawnPoints.Add(spawnPos);
        if (tilemap.HasTile(tilePos) && Random.value < obstacleDensity) CreateObstacle(tilePos, spawnPos, roomType);
        if (tilemap.HasTile(tilePos) && Random.value < oreDensity) SpawnOre(spawnPos, tilePos, roomType);
        if (tilemap.HasTile(tilePos) && Random.value < chestDensity) Instantiate(envLootPrefab[Random.Range(0, envLootPrefab.Length)], spawnPos, Quaternion.identity);
    }
    void CreateObstacle(Vector3 tilePos, Vector3 spawnPos, RoomType roomType)
    {
        GameObject obj = Instantiate(obstaclePrefab[Random.Range(0, obstaclePrefab.Count())], spawnPos, Quaternion.identity);
        Obstacle obs = obj.GetComponent<Obstacle>();
        if (obs != null)
        {
            obs.x = (int)tilePos.x;
            obs.y = (int)tilePos.y;
            obs.mapGenerator = this;
        }
        MakeUnwalkable(tilePos);
    }
    void SpawnOre(Vector3 spawnPos, Vector3 tilePos, RoomType roomType)
    {
        GameObject prefabToSpawn = GetRandomResourceByRarity(DungeonManager.Instance.getFloor());
        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        if (roomType == RoomType.Complex) MakeUnwalkable(tilePos);
    }
    private GameObject GetRandomResourceByRarity(int floorNumber)
    {
        List<(GameObject prefab, float weight)> eligibleResource = new();
        foreach (var entry in orePrefab)
        {
            if (floorNumber >= entry.floorUnlock)
            {
                float adjustedWeight = entry.baseRarityWeight + (floorNumber - entry.floorUnlock) * entry.weightPerFloor;
                eligibleResource.Add((entry.prefab, adjustedWeight));
            }
        }
        float totalWeight = eligibleResource.Sum(e => e.weight);
        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var (prefab, weight) in eligibleResource)
        {
            cumulative += weight;
            if (roll <= cumulative) return prefab;
        }
        return orePrefab[0].prefab; //fallback
    }

    void MakeUnwalkable(Vector3 tilePos)
    {
        if (grid == null)
        {
            pendingUnwalkables.Add(tilePos);
            return;
        }
        int mapX = (int)tilePos.x + -arenaOffset.x;
        int mapY = (int)tilePos.y + -arenaOffset.y;

        if (mapX < 0 || mapY < 0 || mapX >= map.GetLength(0) || mapY >= map.GetLength(1))
        {
            Debug.LogWarning($"MakeUnwalkable out-of-bounds tile: ({mapX}, {mapY}) from tilePos {tilePos}");
            return;
        }

        mapX = Mathf.Clamp(mapX, 0, map.GetLength(0) - 1);
        mapY = Mathf.Clamp(mapY, 0, map.GetLength(1) - 1);

        map[mapX, mapY] = false;

        int gridStartX = mapX * gridResolution;
        int gridStartY = mapY * gridResolution;
        int gridMaxX = grid.GetLength(0);
        int gridMaxY = grid.GetLength(1);

        for (int gridX = gridStartX; gridX < Mathf.Min(gridStartX + gridResolution, gridMaxX); gridX++)
        {
            for (int gridY = gridStartY; gridY < Mathf.Min(gridStartY + gridResolution, gridMaxY); gridY++)
            {
                grid[gridX, gridY].walkable = false;
            }
        }
    }

    public void MakeWalkable(int x, int y)
    {
        map[x, y] = true;
        for (int subX = 0; subX < gridResolution; subX++)
        {
            for (int subY = 0; subY < gridResolution; subY++)
            {
                int gridX = x * gridResolution + subX;
                int gridY = y * gridResolution + subY;
                grid[gridX, gridY].walkable = true;
            }
        }
    }



    // ------------------------------------- Misc -------------------------------------
    public void ClearRoom()
    {
        tilemap.ClearAllTiles();
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        enemySpawnPoints.Clear();
    }
    // get random floor tile
    public Vector3 GenerateRandomPosition()
    {
        List<Vector3Int> floorPositions = new();

        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y]) floorPositions.Add(new Vector3Int(x, y, 0));
            }
        }

        if (floorPositions.Count == 0)
        {
            return Vector3.zero;

        }
        Vector3Int selectedTile = floorPositions[UnityEngine.Random.Range(0, floorPositions.Count)];
        return new Vector3(selectedTile.x + 0.5f, selectedTile.y + 0.5f, 0);
    }
    
}
