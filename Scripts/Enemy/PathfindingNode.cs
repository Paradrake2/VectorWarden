using UnityEngine;

public class PathfindingNode
{
    public int x;
    public int y;
    public bool walkable;
    public float startCost;
    public float hCost; // cost of discovered path
    public float fCost => startCost + hCost;
    public PathfindingNode parent;
    public Vector3 worldPosition;
    public PathfindingNode(int x, int y, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
    }
}
