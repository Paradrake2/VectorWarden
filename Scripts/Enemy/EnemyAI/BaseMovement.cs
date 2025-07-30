using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseMovement : MonoBehaviour
{
    public Transform player;
    public EnemyStats stats;
    public float detectionRange = 10f;
    public float moveSpeed = 5f;
    public bool canMove = true;
    // Pathfinding
    [SerializeField] private MapGenerator mapGenerator;
    private List<PathfindingNode> currentPath;
    private int currentPathIndex = 0;
    private Vector2Int lastPlayerGridPos;
    public float repathThreshold = 0.1f;
    public float colliderMargin = 0.9f;
    public float standoffDistance = 0f;


    protected Rigidbody2D rb;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        mapGenerator = FindFirstObjectByType<MapGenerator>();
    }

    float GetCellSize()
    {
        return 1f / mapGenerator.gridResolution;
    }
    public void Initialize(MapGenerator mapGen)
    {
        mapGenerator = mapGen;

    }
    public bool CanSeePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        Vector2 rayOrigin = (Vector2)transform.position + new Vector2(0f, 0.1f);
        LayerMask obstacleMask = LayerMask.GetMask("Obstacle");

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstacleMask);

        //Debug.DrawRay(rayOrigin, direction * distance, hit.collider == null ? Color.green : Color.red);
        return hit.collider == null;
    }
    protected bool InRange()
    {
        return (GetDistance() <= detectionRange || (GetDistance() >= detectionRange && stats.wasHitByPlayer)) && canMove;
    }
    protected float GetDistance() {
        return Vector2.Distance(transform.position, player.position);
    }
    void OnDrawGizmosSelected()
{
    if (currentPath != null && mapGenerator != null)
    {
        Gizmos.color = Color.magenta;
        float cellSize = 1f / mapGenerator.gridResolution;

        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector3 a = new Vector3(
                (currentPath[i].x + 0.5f) * cellSize + mapGenerator.arenaOffset.x,
                (currentPath[i].y + 0.5f) * cellSize + mapGenerator.arenaOffset.y,
                0);
            Vector3 b = new Vector3(
                (currentPath[i + 1].x + 0.5f) * cellSize + mapGenerator.arenaOffset.x,
                (currentPath[i + 1].y + 0.5f) * cellSize + mapGenerator.arenaOffset.y,
                0);
            Gizmos.DrawLine(a, b);
        }
    }
}

    public virtual void UpdateMovement()
    {
        if (player == null) return;
        if (mapGenerator == null)
        {
            mapGenerator = FindFirstObjectByType<MapGenerator>();
            if (mapGenerator == null)
            {
                Debug.LogError("MapGenerator not found in scene!");
                return;
            }
        }
        if (InRange())
        {
            if (CanSeePlayer())
            {
                BeelineTowardsPlayer();
            }
            else
            {
                Pathfind();
                //animator.SetBool("isRunning", true);
            }
        }
        else
        {
            //animator.SetBool("isRunning", false);
            currentPath = null;
        }
    }
    protected void Pathfind()
    {
        Vector2Int currentPlayerGridPos = GetNearestWalkableCell(player.position);

        // Check if need to recalculate path
        if (currentPath == null || currentPath.Count == 0 || Vector2Int.Distance(currentPlayerGridPos, lastPlayerGridPos) > repathThreshold)
        {
            Vector2Int startPos = WorldToGridPosition(transform.position);
            if (!IsWalkable(startPos)) return;
            currentPath = FindPath(startPos, currentPlayerGridPos);
            currentPathIndex = 0;
            lastPlayerGridPos = currentPlayerGridPos;
        }

        MoveAlongPath();
    }
    protected Vector3 Direction()
    {
        return (player.position - transform.position).normalized;
    }
    protected void BeelineTowardsPlayer()
    {
        transform.position += Direction() * stats.getMovementSpeed() * Time.deltaTime;

        //HandleSpriteDirection(Direction());
        //animator.SetBool("isRunning", true);
    }

    protected Vector2Int WorldToGridPosition(Vector3 worldPos) {

    if (mapGenerator == null) Debug.LogWarning("MapGenerator is null");
    if (mapGenerator.grid == null) Debug.LogWarning("Grid is null");

    // Adjust world position by arenaOffset
        float offsetX = worldPos.x - mapGenerator.arenaOffset.x;
    float offsetY = worldPos.y - mapGenerator.arenaOffset.y;

    int x = Mathf.FloorToInt(offsetX * mapGenerator.gridResolution);
    int y = Mathf.FloorToInt(offsetY * mapGenerator.gridResolution);

    // Clamp to grid bounds
    int maxX = mapGenerator.grid.GetLength(0) - 1;
    int maxY = mapGenerator.grid.GetLength(1) - 1;

    x = Mathf.Clamp(x, 0, maxX);
    y = Mathf.Clamp(y, 0, maxY);

    return new Vector2Int(x, y);
    }
    public List<PathfindingNode> FindPath(Vector2Int startPos, Vector2Int targetPos)
    {
        var grid = mapGenerator.grid;

        foreach (var node in grid)
        {
            node.startCost = float.MaxValue;
            node.hCost = 0;
            node.parent = null;
        }

        PathfindingNode startNode = grid[startPos.x, startPos.y];
        PathfindingNode targetNode = grid[targetPos.x, targetPos.y];

        List<PathfindingNode> openSet = new() { startNode };
        HashSet<PathfindingNode> closedSet = new();

        startNode.startCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        startNode.parent = null;

        while (openSet.Count > 0)
        {
            PathfindingNode bestNode = openSet[0];
            float bestCost = bestNode.startCost + bestNode.hCost;

            foreach (var node in openSet)
            {
                float cost = node.startCost + node.hCost;
                if (cost < bestCost)
                {
                    bestNode = node;
                    bestCost = cost;
                }
            }

            if (bestNode == targetNode) return RetracePath(startNode, targetNode);

            openSet.Remove(bestNode);
            closedSet.Add(bestNode);

            foreach (PathfindingNode neighbor in GetNeighbors(bestNode, grid))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) continue;

                float tempStart = bestNode.startCost + GetDistance(bestNode, neighbor);
                if (tempStart < neighbor.startCost || !openSet.Contains(neighbor))
                {
                    neighbor.startCost = tempStart;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = bestNode;

                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }
        return null;
    }
    Vector2Int GetNearestWalkableCell(Vector3 worldPos)
    {
        Vector2Int gridPos = WorldToGridPosition(worldPos);
        if (IsWalkable(gridPos)) return gridPos;

        // Simple fallback: search 3x3 neighboring subcells (you can expand this)
        int[] dx = { 0, 1, -1, 0, 0, 1, 1, -1, -1 };
        int[] dy = { 0, 0, 0, 1, -1, 1, -1, 1, -1 };

        for (int i = 0; i < dx.Length; i++)
        {
            Vector2Int neighbor = new Vector2Int(gridPos.x + dx[i], gridPos.y + dy[i]);
            if (IsWalkable(neighbor)) return neighbor;
        }

        // As last resort, just return original snapped position
        return gridPos;
    }

    protected bool IsWalkable(Vector2Int gridPos)
{
    if (mapGenerator == null || mapGenerator.grid == null)
        return false;

    int gridWidth = mapGenerator.grid.GetLength(0);
    int gridHeight = mapGenerator.grid.GetLength(1);

    // Calculate how many cells the enemy covers
    float cellSize = 1f / mapGenerator.gridResolution;
    int enemyCellsWide = Mathf.CeilToInt(stats.colliderWidth / cellSize * colliderMargin);
    int enemyCellsHigh = Mathf.CeilToInt(stats.colliderHeight / cellSize * colliderMargin);

    // Check all cells the enemy would occupy
    for (int x = gridPos.x; x < gridPos.x + enemyCellsWide; x++)
    {
        for (int y = gridPos.y; y < gridPos.y + enemyCellsHigh; y++)
        {
            if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight)
                return false;
            if (!mapGenerator.grid[x, y].walkable)
                return false;
        }
    }
    return true;
}
    protected void MoveAlongPath()
{
    if (currentPath == null || currentPathIndex >= currentPath.Count) return;

    float cellSize = 1f / mapGenerator.gridResolution;
    Vector3 targetWorldPos;

    if (currentPathIndex == currentPath.Count - 1)
    {
        // Final node: move directly to player
        targetWorldPos = new Vector3(player.position.x, player.position.y, 0);
    }
    else
    {
        PathfindingNode node = currentPath[currentPathIndex];
        targetWorldPos = new Vector3(
            (node.x + 0.5f) * cellSize + mapGenerator.arenaOffset.x,
            (node.y + 0.5f) * cellSize + mapGenerator.arenaOffset.y,
            0);
    }

    Vector3 direction = (targetWorldPos - transform.position).normalized;
    transform.position += direction * stats.getMovementSpeed() * Time.deltaTime;

    if (Vector3.Distance(transform.position, targetWorldPos) < 0.1f)
    {
        currentPathIndex++;
    }
}

    float GetDistance(PathfindingNode a, PathfindingNode b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        int straight = Mathf.Abs(dx - dy);
        int diagonal = Mathf.Min(dx, dy);

        return 1.4f * diagonal + 1f * straight;
    }

    List<PathfindingNode> RetracePath(PathfindingNode start, PathfindingNode end)
    {
        List<PathfindingNode> path = new();
        PathfindingNode current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    List<PathfindingNode> GetNeighbors(PathfindingNode node, PathfindingNode[,] grid)
{
    List<PathfindingNode> neighbors = new();
    int[] dx = { -1, 1, 0, 0, -1, -1, 1, 1 };
    int[] dy = { 0, 0, -1, 1, -1, 1, -1, 1 };

    for (int i = 0; i < dx.Length; i++)
    {
        int nx = node.x + dx[i];
        int ny = node.y + dy[i];
        if (nx >= 0 && ny >= 0 && nx < grid.GetLength(0) && ny < grid.GetLength(1))
        {
            Vector2Int neighborPos = new Vector2Int(nx, ny);
            // Only add if the enemy can fully fit in this cell
            if (IsWalkable(neighborPos))
                neighbors.Add(grid[nx, ny]);
        }
    }
    return neighbors;
}
    // Update is called once per frame
    void Update()
    {
        
    }
}
