using UnityEngine;

public class BossProjectileFactory : MonoBehaviour
{
    public void FireProjectile(EnemyProjectile projectilePrefab, Vector3 shooterPos, Vector3 targetPos, Vector2 targetVelocity)
    {
        GameObject projectile = Instantiate(projectilePrefab.gameObject, shooterPos, Quaternion.identity);
        EnemyProjectile projData = projectile.GetComponent<EnemyProjectile>();
        projData.speed = projectile.GetComponent<EnemyProjectile>().speed;
        projData.damage = projectile.GetComponent<EnemyProjectile>().damage;

        float timeToIntercept = GetPredictedPosition(shooterPos, targetPos, targetVelocity, projData.speed);
        if (timeToIntercept > 0)
        {
            Vector3 targetPosition = targetPos + (Vector3)(targetVelocity * timeToIntercept);
            Vector3 direction = (targetPosition - shooterPos).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projData.speed;
            projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
        else
        {
            Vector3 direction = (targetPos - shooterPos).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * projData.speed;
            projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }
    public float GetPredictedPosition(Vector3 shooterPos, Vector3 targetPos, Vector2 targetVelocity, float projectileSpeed)
    {
        Vector2 displacement = targetPos - shooterPos;
        float a = targetVelocity.sqrMagnitude - projectileSpeed * projectileSpeed;
        float b = 2f * Vector2.Dot(displacement, targetVelocity);
        float c = displacement.sqrMagnitude;

        float disc = b * b - (4f * a * c);
        if (disc < 0) return -1f; // No valid solution

        float sqrtDisc = Mathf.Sqrt(disc);
        float t1 = (-b + sqrtDisc) / (2f * a);
        float t2 = (-b - sqrtDisc) / (2f * a);

        float t = Mathf.Min(t1, t2);
        if (t < 0f) t = Mathf.Max(t1, t2);
        return t > 0f ? t : -1f; // Return the positive time or -1 if no valid time
    }
}
