using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteGenerator : MonoBehaviour
{
    public static SpriteGenerator Instance;
    public GameObject test;
    Dictionary<string, Queue<Color>> colorDict = new();
    public Camera renderCam;
    public RenderTexture renderTexture;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (renderCam == null)
    {
        GameObject camObj = GameObject.FindGameObjectWithTag("IconCaptureCamera");
        if (camObj != null)
        {
            renderCam = camObj.GetComponent<Camera>();
        }
        else
        {
            Debug.LogError("Render camera not found! Make sure it's tagged 'RenderCam'");
        }
    }
    }

    void AssignColor(List<Items> ingredients, List<EquipmentPartSprite> parts) {
        bool[] used = new bool[ingredients.Count];
        foreach (var part in parts) {
            bool matched = false;
            for (int i = 0; i < ingredients.Count; i++) {
                if (!used[i] && ingredients[i].tags.Contains(part.partTag)) {
                    ingredients[i].color.a = 1f;
                    part.Renderer.color = ingredients[i].color;
                    used[i] = true;
                    matched = true;
                    break;
                }
            }
            if (!matched) {
                Debug.LogError("No unused ingredient");
            }
        }
    }
    public Sprite GenerateIcon(GameObject equipmentVisual, List<Items> ingredients) {
        if (equipmentVisual == null) {
            Debug.LogError("PREFAB REF IS NULL");
        }
        GameObject prefabInstance = Instantiate(equipmentVisual, new Vector3(1000,1000,0), Quaternion.identity);
        List<EquipmentPartSprite> parts = prefabInstance.GetComponentsInChildren<EquipmentPartSprite>(true).ToList();
        prefabInstance.SetActive(true);
        AssignColor(ingredients, parts);
        SetLayerRecursively(prefabInstance, LayerMask.NameToLayer("IconCapture"));

        // Ensure the camera sees it
        renderCam.transform.position = new Vector3(1000, 1000, -10);
        renderCam.orthographic = true;
        renderCam.orthographicSize = 1.0f;
        renderCam.clearFlags = CameraClearFlags.SolidColor;
        renderCam.backgroundColor = new Color(0, 0, 0, 0);
        renderCam.cullingMask = 1 << LayerMask.NameToLayer("IconCapture");

        // Assign targetTexture explicitly
        renderCam.targetTexture = renderTexture;

        // Render and read
        renderCam.Render();

        RenderTexture.active = renderTexture;
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;

        //Destroy(prefabInstance);
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
