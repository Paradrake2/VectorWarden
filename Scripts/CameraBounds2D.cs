using UnityEngine;

public static class CameraBounds2D
{
    public static Rect GetWorldRect(Camera cam)
    {
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        Vector3 camPos = cam.transform.position;
        return new Rect(camPos.x - width / 2, camPos.y - height / 2, width, height);
    }

    public static bool IsInBounds(Vector3 position, Camera cam)
    {
        Rect worldRect = GetWorldRect(cam);
        return worldRect.Contains(new Vector2(position.x, position.y));
    }

    
}
