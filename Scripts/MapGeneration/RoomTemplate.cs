using UnityEngine;

public enum RoomType
{
    Simple,
    Complex,
    Boss
}

public enum RoomLayout
{
    Cellular,
    Arena,
    Maze,
    Corridor,
    Boss
}
[CreateAssetMenu(fileName = "RoomTemplate", menuName = "Scriptable Objects/RoomTemplate")]
public class RoomTemplate : ScriptableObject
{
    public string templateName;
    public int maxHeight = 40;
    public int minHeight = 20;
    public int maxWidth = 40;
    public int minWidth = 20;
    public int maxRadius = 40;
    public int minRadius = 20;
    public RoomType roomType;
    public RoomLayout roomLayout;
    public float noise;
    public int iterations;
    public float maxEnemyDensity = 0.3f;
    public float maxOreDensity = 0.1f;
    public float maxChestDensity = 0.05f;
    public float maxObstacleWeight = 0.1f;
    public float minEnemyDensity = 0f;
    public float minOreDensity = 0f;
    public float minChestDensity = 0f;
    public float minObstacleWeight = 0f;

}
