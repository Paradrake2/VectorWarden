using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BossAttackComponent", menuName = "BossComponents/BossAttackComponent")]
public abstract class BossAttackComponent : ScriptableObject
{
    BossProjectileFactory bp = FindFirstObjectByType<BossProjectileFactory>();
    public abstract IEnumerator ExecuteAttack(BossProjectileFactory bp);
}
