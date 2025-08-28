using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "AutoAttack/Aura")]
public class AuraAttackSO : AutoAttackData
{
    public float radius = 5f;
    public float auraVisualDuration = 0.2f;
    public override IEnumerator Execute(AutoAttackContext ctx)
    {
        var ps = ctx.PlayerStats;
        Collider2D[] hits = Physics2D.OverlapCircleAll(ctx.Origin.position, radius);
        AuraVisual(ctx.Origin);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var es = hit.GetComponent<EnemyStats>();
                if (es != null)
                {
                    es.TakeDamage(ps.GetAutoAttackDamage());
                }
            }
        }
        Debug.LogError("AuraAttackSO Execute not implemented yet.");
        yield break;
    }
    void AuraVisual(Transform origin)
    {
        var go = Instantiate(projectilePrefab, origin.position, Quaternion.identity);
        go.transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
        Destroy(go, auraVisualDuration);
    }
}
