using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AutoAttack/Orbital")]
public class OrbitalSO : AutoAttackData
{
    public float orbitRadius = 4f;
    public int startingProjCount = 2;
    public float rotationSpeed = 180f; // degrees per second
    public float regenCooldown = 0.75f; // how many seconds between projectile regenerations

    public bool useUpgradeTierPrefab = true; // in case I want to use the upgraded tier prefab
    public override IEnumerator Execute(AutoAttackContext ctx)
    {
        var controller = OrbitalAttack.Instance;
        var pp = projectilePrefab.GetComponent<PlayerProjectile>();
        var data = pp.projectileData;
        int level = ProjectileLevelTracker.Instance.GetLevel(data);

        // Null check
        if (controller == null)
        {
            controller = ctx.Origin.GetComponent<OrbitalAttack>();
            if (controller == null)
            {
                controller = ctx.Origin.gameObject.AddComponent<OrbitalAttack>();
            }
        }

        GameObject prefabToUse = projectilePrefab;
        if (useUpgradeTierPrefab)
        {

            if (data != null)
            {
                var tierPrefab = data.projectileUpgrade.tiers[level].projectilePrefab;
                if (tierPrefab != null)
                {
                    prefabToUse = tierPrefab;
                }
            }
        }

        int maxProjectiles = startingProjCount + (data?.projectileUpgrade?.tiers[level].projectileAdd ?? 0) + PlayerStats.Instance.GetOrbitalProjectileNum();

        controller.AddOrUpdateOrbitals(prefabToUse, orbitRadius, maxProjectiles, rotationSpeed, regenCooldown);
        yield break;
    }
}
