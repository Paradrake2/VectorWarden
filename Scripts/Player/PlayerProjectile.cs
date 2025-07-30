using UnityEngine;
public enum ProjectileType
{
    Normal,
    Explosive,
    Piercing,
    Homing
}
public class PlayerProjectile : MonoBehaviour
{
    public PlayerStats playerStats;
    public float projectileSpeed;
    void Start()
    {
        projectileSpeed = playerStats.GetProjectileSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
