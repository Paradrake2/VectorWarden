using UnityEngine;

[CreateAssetMenu(fileName = "MaterialVisualData", menuName = "Scriptable Objects/MaterialVisualData")]
public class MaterialVisualData : ScriptableObject
{
    public MaterialRoleType role;
    public string materialId;
    public int index;
    public string materialTag;
    public Color tintColor;
    public Sprite overrideSprite;
}
