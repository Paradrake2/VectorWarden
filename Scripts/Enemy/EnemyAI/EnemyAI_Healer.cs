using UnityEngine;


// -------- CAUSES IMMENSE LAG --------
public class EnemyAI_Healer : MonoBehaviour
{
    [Header("Healing")]
    public EnemyStats enemyStats;
    public float healPrefabLifetime = 0.5f;
    public GameObject healEffectPrefab;

    [Header("Query")]
    public LayerMask enemyLayer;          // set to your Enemy layer in Inspector
    public int maxHits = 64;              // cap for results buffer

    // --- internals ---
    private Collider2D[] _hits;           // reused buffer (no GC)
    private ContactFilter2D _filter;      // reused filter (no GC)
    private float _healTimer;

    // (optional) small de-sync so many healers don't all fire same frame
    [SerializeField] private bool desyncStart = true;

    void Awake()
    {
        if (!enemyStats) enemyStats = GetComponent<EnemyStats>();

        _hits = new Collider2D[maxHits];

        _filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = enemyLayer,
            useTriggers = false
        };

        _healTimer = desyncStart ? Random.value * enemyStats.healCooldown : 0f;
    }

    void Update()
    {
        _healTimer -= Time.deltaTime;
        if (_healTimer <= 0f)
        {
            HealEnemies_NoAlloc();
            _healTimer = enemyStats.healCooldown;
        }
    }

    private void HealEnemies_NoAlloc()
    {
        // NonAlloc query: writes into _hits; returns count
        int count = Physics2D.OverlapCircle(transform.position, enemyStats.healRadius, _filter, _hits);

        // Visual without coroutine/GC: use a timed helper (or pool; see below)
        SpawnHealEffect();

        for (int i = 0; i < count; i++)
        {
            var c = _hits[i];
            // Fast tag / component checks; TryGetComponent avoids exceptions
            if (c.CompareTag("Enemy") && c.TryGetComponent(out EnemyStats target)
                && target != enemyStats && target.type != EnemyType.Healer)
            {
                target.AddHealth(enemyStats.healAmount);
            }
        }
    }

    private void SpawnHealEffect()
    {
        if (!healEffectPrefab) return;

        // OPTION A: reuse one effect object (no coroutine, no destroy)
        // If your effect is a particle system, you can play & let it auto-stop.
        var fx = HealEffectPool.Get(healEffectPrefab, transform.position);
        fx.transform.localScale = new Vector3(enemyStats.healRadius, enemyStats.healRadius, 1f);
        TimedReturn.ToPool(fx, healPrefabLifetime); // see helper below
    }

    // (Optional) quick debug gizmo
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.6f, 0.35f);
        if (enemyStats) Gizmos.DrawWireSphere(transform.position, enemyStats.healRadius);
    }
}
