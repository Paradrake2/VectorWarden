using UnityEngine;
public static class CamEdge
{
    public static Vector3 GetTopLeft(Camera cam)
    {
        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
        return topLeft;
    }

    public static Vector3 GetTopRight(Camera cam)
    {
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        return topRight;
    }

    public static Vector3 GetBottomLeft(Camera cam)
    {
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        return bottomLeft;
    }

    public static Vector3 GetBottomRight(Camera cam)
    {
        Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        return bottomRight;
    }
}
