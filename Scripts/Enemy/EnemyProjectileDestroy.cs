using UnityEngine;

public class EnemyProjectileDestroy : MonoBehaviour
{
    // The point of this class is to destroy enemy projectiles after a certain time to prevent them from lingering indefinitely
    public float lifetime = 5f; // seconds
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
