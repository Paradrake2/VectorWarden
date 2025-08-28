using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalSO : AutoAttackData
{
    public float orbitRadius = 4f;
    public int startingProjCount = 2;
    public float rotationSpeed = 180f; // degrees per second
    public float regenCooldown = 0.75f; // how many seconds between projectile regenerations
    private readonly List<GameObject> activeProjectiles = new List<GameObject>();
    private float regenTimer;
    private bool hasOrbitals = false;
    private void Start() {
        
    }
    public override IEnumerator Execute(AutoAttackContext ctx)
    {
        Debug.LogError("OrbitalSO Execute not implemented yet.");
        yield break;
    }
}
