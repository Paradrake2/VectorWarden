using UnityEngine;

public class BossProjectileFactory : MonoBehaviour
{
    public void FireProjectile(EnemyProjectile projectilePrefab, Vector3 shooterPos3, Vector3 targetPos3, Vector2 targetVelocityWS)
    {
        // Work in XY space
        Vector2 shooterPos = (Vector2)shooterPos3;
        Vector2 targetPos = (Vector2)targetPos3;

        GameObject projectile = Instantiate(projectilePrefab.gameObject, shooterPos3, Quaternion.identity);
        var proj = projectile.GetComponent<EnemyProjectile>();
        var rb = projectile.GetComponent<Rigidbody2D>();

        float shotSpeed = Mathf.Max(0.0001f, proj.speed);

        // 1) Analytic lead time
        float t = ComputeInterceptTime(shooterPos, targetPos, targetVelocityWS, shotSpeed);

        // 2) If analytic fails (t < 0 or NaN), do a quick 2–3 step fixed-point iterate (robust in practice)
        if (!(t > 0f) || float.IsNaN(t))
        {
            t = IterativeLeadTime(shooterPos, targetPos, targetVelocityWS, shotSpeed, 3);
        }

        // 3) If still bad, clamp to 0 so we at least fire forward
        if (t < 0f || float.IsNaN(t)) t = 0f;

        Vector2 aimPoint = targetPos + targetVelocityWS * t;
        Vector2 dir = (aimPoint - shooterPos).normalized;

        // Unity 6: velocity => linearVelocity
        rb.linearVelocity = dir * shotSpeed;

        // 2D-friendly facing: rotate so 'up' points along dir (use 'right' if your sprite points right)
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        rb.SetRotation(ang);

        // Helpful debug
        float a, b, c, disc, t1, t2;
        DebugLead(shooterPos, targetPos, targetVelocityWS, shotSpeed, out a, out b, out c, out disc, out t1, out t2);
        Debug.Log($"[Lead] t={t:F3}, vT={targetVelocityWS} | a={a:F3} b={b:F3} c={c:F3} disc={disc:F3} roots=({t1:F3},{t2:F3})");
        Debug.DrawLine(shooterPos, aimPoint, Color.cyan, 1.5f);
    }

    // Analytic lead-time solver in 2D:
    // ||r + v t|| = s t  ->  (v·v - s^2)t^2 + 2(r·v)t + r·r = 0
    private float ComputeInterceptTime(Vector2 shooterPos, Vector2 targetPos, Vector2 v, float s)
    {
        Vector2 r = targetPos - shooterPos;
        float v2 = Vector2.Dot(v, v);
        float s2 = s * s;

        float a = v2 - s2;
        float b = 2f * Vector2.Dot(r, v);
        float c = Vector2.Dot(r, r);

        if (Mathf.Abs(a) < 1e-6f)
        {
            if (Mathf.Abs(b) < 1e-6f) return -1f;
            float tLin = -c / b;                 // linear case
            return tLin > 0f ? tLin : -1f;
        }

        float disc = b * b - 4f * a * c;
        if (disc < 0f) return -1f;

        float sqrtDisc = Mathf.Sqrt(disc);
        float t1 = (-b - sqrtDisc) / (2f * a);
        float t2 = (-b + sqrtDisc) / (2f * a);

        if (t1 > 0f && t2 > 0f) return Mathf.Min(t1, t2);
        if (t1 > 0f) return t1;
        if (t2 > 0f) return t2;
        return -1f;
    }

    // Simple fixed-point iterate: start with TOF ≈ distance/speed, update with the led position.
    private float IterativeLeadTime(Vector2 shooterPos, Vector2 targetPos, Vector2 v, float s, int iterations = 3)
    {
        float t = Vector2.Distance(shooterPos, targetPos) / Mathf.Max(0.0001f, s);
        for (int i = 0; i < iterations; i++)
        {
            Vector2 future = targetPos + v * t;
            float newT = Vector2.Distance(shooterPos, future) / Mathf.Max(0.0001f, s);
            if (Mathf.Abs(newT - t) < 1e-3f) break;
            t = newT;
        }
        return float.IsFinite(t) ? t : -1f;
    }

    private void DebugLead(Vector2 shooterPos, Vector2 targetPos, Vector2 v, float s,
    out float a, out float b, out float c, out float disc, out float t1, out float t2)
    {
        Vector2 r = targetPos - shooterPos;
        float v2 = Vector2.Dot(v, v);
        float s2 = s * s;
        a = v2 - s2;
        b = 2f * Vector2.Dot(r, v);
        c = Vector2.Dot(r, r);
        disc = b * b - 4f * a * c;
        if (disc >= 0f && Mathf.Abs(a) >= 1e-6f)
        {
            float sqrtDisc = Mathf.Sqrt(disc);
            t1 = (-b - sqrtDisc) / (2f * a);
            t2 = (-b + sqrtDisc) / (2f * a);
        }
        else { t1 = -1f; t2 = -1f; }
    }
    public float DebugPredictTOF(Vector2 shooterPos, Vector2 targetPos, Vector2 v, float s)
    {
        return ComputeInterceptTime(shooterPos, targetPos, v, s);
    }
}
