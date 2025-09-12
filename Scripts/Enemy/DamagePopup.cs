using UnityEngine;
using TMPro;
using System.Collections;
public class DamagePopup : MonoBehaviour
{
    [Header("Refs")]
    public TextMeshPro tmp;

    [Header("Settings")]
    public float lifetime = 1.5f;
    public Vector3 startVelocity = new Vector3(0, 1.2f, 0);
    public float gravity = 0f; // 0 for gentle float, >0 to arc down
    public float fadeStart = 0.7f;
    public bool faceCamera = true;
    public int sortingOrder = 10000; // draw on top of everything

    [Header("TMP Settings")]
    public float fontSize = 8f;

    float _time;
    Vector3 _velocity;
    Camera _camera;
    Color _color;
    float _t;

    public static void Spawn(float amount, Vector3 worldPos, Color color)
    {
        var go = new GameObject("DamagePopup");
        go.transform.position = worldPos + new Vector3(0, 0, -0.1f); // Offset to avoid z-fighting

        var popup = go.AddComponent<DamagePopup>();
        popup._camera = Camera.main;
        popup._color = color;

        var tmp = go.AddComponent<TextMeshPro>();
        popup.tmp = tmp;

        if (TMP_Settings.defaultFontAsset != null)
        {
            tmp.font = TMP_Settings.defaultFontAsset;
        }

        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = popup.fontSize;
        tmp.color = color;
        tmp.text = amount.ToString();
        tmp.ForceMeshUpdate();

        var mr = tmp.renderer;
        mr.sortingOrder = popup.sortingOrder;

        popup._velocity = popup.startVelocity;
        popup.StartCoroutine(popup.Run());
    }

    IEnumerator Run()
    {
        while (_t < lifetime)
        {
            _t += Time.deltaTime;

            _velocity.y -= gravity * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;

            if (faceCamera && _camera)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
            }

            float u = Mathf.Clamp01(_t / lifetime);
            transform.localScale = Vector3.one * Mathf.Lerp(1.1f, 0.95f, u);

            if (u >= fadeStart)
            {
                float f = Mathf.InverseLerp(fadeStart, 1f, u);
                var c = _color; c.a = 1f - f;
                tmp.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}
