using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyQuery
{
    // Define methods for querying enemies in the scene
    // For example:
    // List<Enemy> GetEnemiesInRange(Vector3 position, float range);
    // Enemy GetClosestEnemy(Vector3 position);
    IEnumerable<Transform> GetAllEnemies();
}

public class SceneEnemyQuery : IEnemyQuery
{
    public IEnumerable<Transform> GetAllEnemies()
    {
        // Find all enemy objects in the scene
        EnemyStats[] enemies = GameObject.FindObjectsByType<EnemyStats>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            yield return enemy.transform;
        }
    }
}
