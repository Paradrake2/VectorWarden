using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "BossAttackComponent", menuName = "BossComponents/BossAttackComponent")]
public abstract class BossAttackComponent : ScriptableObject
{
    public Player player;
    public Boss boss;
    public abstract IEnumerator ExecuteAttack(BossProjectileFactory bp);
}
