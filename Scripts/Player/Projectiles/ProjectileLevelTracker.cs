using System.Collections.Generic;
using UnityEngine;

public class ProjectileLevelTracker : MonoBehaviour
{
    public static ProjectileLevelTracker Instance { get; private set; }

    [SerializeField] private Dictionary<ProjectileData, int> projectileLevels = new();

    public List<ProjectileUpgrade> upgradeAssets;
    [SerializeField] private Dictionary<ProjectileData, ProjectileUpgrade> _projectileUpgrades = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        

        foreach (var upgrade in upgradeAssets)
        {
            if (upgrade.projectileData != null)
            {
                _projectileUpgrades[upgrade.projectileData] = upgrade;
            }
        }
    }
    public void ResetLevels()
    {
        // Create a copy of the keys to avoid modifying dictionary while iterating
        var keys = new List<ProjectileData>(projectileLevels.Keys);
        
        foreach (var key in keys)
        {
            projectileLevels[key] = 0;
        }
    }

    public int GetLevel(ProjectileData proj)
    {
        return projectileLevels.TryGetValue(proj, out int level) ? level : 0;
    }

    public void AddLevel(ProjectileData proj, int delta = 1)
    {
        int current = GetLevel(proj);
        var u = _projectileUpgrades.TryGetValue(proj, out var up) ? up : null;
        int max = u ? Mathf.Max(1, up.maxLevel) : int.MaxValue;

        int next = Mathf.Clamp(current + delta, 1, max);
        projectileLevels[proj] = next;

        if (u != null)
        {
            var tier = u.GetTier(next);
            //if (tier != null && tier.onLevelUpSfx) AudioSource.PlayClipAtPoint(tier.onLevelUpSfx, transform.position); // Sound on level up
        }
    }

    public ProjectileUpgrade.Tier GetTier(ProjectileData proj)
    {
        if (!_projectileUpgrades.TryGetValue(proj, out var up)) return null;
        return up.GetTier(GetLevel(proj));
    }
}
