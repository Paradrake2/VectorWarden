using UnityEngine;

public class TelegraphScope : System.IDisposable
{
    LineRenderer lr;
    Transform owner, target;

    public TelegraphScope(Transform owner, Transform target)
    {
        this.owner = owner;
        this.target = target;
        var go = new GameObject("AimLine");
        lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.widthMultiplier = 0.05f;
        lr.useWorldSpace = true;
        lr.numCapVertices = 4;
    }

    public void UpdateLine(Vector2 from, Vector2 to, bool lockedPhase)
    {
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        if (lockedPhase)
        {
            lr.startColor = Color.red;
            lr.endColor = Color.red;
        }
        else
        {
            lr.startColor = Color.yellow;
            lr.endColor = Color.yellow;
        }
    }
    public void Dispose()
    {
        if (lr) GameObject.Destroy(lr.gameObject);
    }
}
