using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class OrbitalAttack : MonoBehaviour
{
    public static OrbitalAttack Instance { get; private set; }
    public PlayerStats playerStats;
    public Player player;
    public List<GameObject> orbitalPrefabs = new List<GameObject>();

    public int maxProjectiles = 2; // default number of projectiles
    public float radius = 1.8f; // offset from player
    public float rotationSpeed = 180f; // degrees per second
    public float regenCooldown = 0.75f; // how many seconds between projectile regenerations

    private readonly List<GameObject> activeProjectiles = new List<GameObject>();
    private bool regenRunning;
    private float baseAngle;
    private float regenTimer;
    private bool hasOrbitals = false;
    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }
        Instance = this;
    }
    void FindPlayer()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
        }
    }
    void OnEnable()
    {
        FindPlayer();
        regenRunning = true;
        StartCoroutine(RegenerateProjectiles());
    }
    void OnDisable()
    {
        StopAllCoroutines();
        foreach (var go in activeProjectiles) if (go) Destroy(go);
        activeProjectiles.Clear();
    }

    IEnumerator RegenerateProjectiles()
    {
        while (regenRunning)
        {
            if (hasOrbitals)
            {
                while (activeProjectiles.Count < maxProjectiles)
                {
                    SpawnOne(orbitalPrefabs[0]);
                    yield return null; // Wait for the next frame
                }
                yield return new WaitForSeconds(regenCooldown); // Wait for the next frame
            }
            else
            {
                yield return null;
            }
        }
    }
    void SpawnOne(GameObject orbitalPrefab)
    {
        FindPlayer();
        float damage = playerStats.CurrentDamage * playerStats.GetAutoAttackDamageMultiplier();
        int pierce = Mathf.CeilToInt(damage / 5f) + playerStats.GetPiercingAmount();

        GameObject newProjectile = Instantiate(orbitalPrefab, transform.position, Quaternion.identity);
        PlayerProjectile projData = newProjectile.GetComponent<PlayerProjectile>();

        projData.InitializeProjectile(Vector2.zero, 0, damage, ProjectileType.Orbital);
        projData.pierceAmount = pierce;
        projData.SetOrbitalOwner(this);

        FindPlayer(); // in case player is null for whatever reason
        activeProjectiles.Add(newProjectile);
        regenTimer = regenCooldown;
    }
    public void OnOrbitalExpired(PlayerProjectile p)
    {
        if (p != null) activeProjectiles.Remove(p.gameObject);
    }
    void GetPrefabs()
    {
        foreach (var autoAttack in playerStats.autoAttackDataList)
        {
            if (autoAttack.attackType != AutoAttackType.Orbital) continue;
        
            maxProjectiles = autoAttack.projectileCount;
            hasOrbitals = true;
            Debug.Log("Has orbitals");
            if (!orbitalPrefabs.Contains(autoAttack.projectilePrefab) && autoAttack.projectilePrefab != null)
                orbitalPrefabs.Add(autoAttack.projectilePrefab);

        }
    }
    void Update()
    {
        GetPrefabs();
        activeProjectiles.RemoveAll(item => item == null);
        if (orbitalPrefabs.Count == 0) return;
        baseAngle += rotationSpeed * Time.deltaTime;
        Vector3 playerPos = player.transform.position;

        int n = activeProjectiles.Count;
        if (n == 0) return;

        float step = 360f / n;
        for (int i = 0; i < n; i++)
        {
            float ang = baseAngle + i * step;
            Vector3 offset = new Vector3(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad), 0) * radius;
            Transform t = activeProjectiles[i].transform;
            t.localPosition = playerPos + offset;
            t.rotation = Quaternion.Euler(0, 0, ang);
        }
    }
}
