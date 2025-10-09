using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "FireProjectile", menuName = "BossComponents/BossAttackComponents/FireProjectile")]
public class FireProjectile : BossAttackComponent
{
    public EnemyProjectile projectilePrefab;

    public Vector2 reactionDelayRange = new Vector2(80f, 120f);
    public bool trackDuringDelay = true;
    public bool lockLast60ms = true;
    public override IEnumerator ExecuteAttack(BossProjectileFactory bp = null)
    {
        Setup();
        if (bp == null)
        {
            bp = FindFirstObjectByType<BossProjectileFactory>();
        }
        if (bp == null)
        {
            Debug.LogError("Boss projectile factory not found.");
        }
        var pm = player.GetComponent<PlayerMovement>();
        float delay = Random.Range(reactionDelayRange.x, reactionDelayRange.y) / 1000f;
        Vector2 lockedTargetVel = pm ? pm.SmoothedVelocity : Vector2.zero;

        using (var telegraph = new TelegraphScope(boss.transform, player.transform))
        {
            float timer = 0f;
            float lockTime = (lockLast60ms && delay > 0.06f) ? delay - 0.06f : delay;

            Vector3 lockedTargetPos = player.transform.position;

            while (timer < delay)
            {
                bool lockedPhase = (timer >= lockTime);

                Vector3 tgtPos = lockedPhase && !trackDuringDelay ? lockedTargetPos : player.transform.position;
                Vector2 tgtVel = lockedPhase && !trackDuringDelay ? lockedTargetVel : (pm ? pm.SmoothedVelocity : Vector2.zero);

                // Draw the line to the predicted position
                Vector2 shooter = boss.transform.position;
                Vector2 target = (Vector2)tgtPos;
                float speed = projectilePrefab.speed;

                float t = bp.DebugPredictTOF(shooter, target, tgtVel, speed);
                if (t < 0f) t = 0f;

                Vector2 aimPoint = target + tgtVel * t;
                telegraph.UpdateLine(shooter, aimPoint, lockedPhase);

                if (lockedPhase && trackDuringDelay)
                {
                    lockedTargetPos = tgtPos;
                    lockedTargetVel = tgtVel;
                }

                timer += Time.deltaTime;
                yield return null;
            }
        }

        var pmFinal = player.GetComponent<PlayerMovement>();
        Vector2 vt = (trackDuringDelay || !lockLast60ms) ? (pmFinal ? pmFinal.SmoothedVelocity : Vector2.zero) : lockedTargetVel;


        bp.FireProjectile(projectilePrefab, boss.transform.position, player.transform.position, vt);
        yield return null;
    }
    void Setup()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        if (boss == null)
        {
            boss = FindFirstObjectByType<Boss>();
        }
    }
}
