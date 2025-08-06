using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;


[System.Serializable]
public class ComponentSlot
{
    public string slotName;
    public string acceptedTag;
    public int tier;
}


[CreateAssetMenu(fileName = "Blueprint", menuName = "Scriptable Objects/Blueprint")]
public class Blueprint : ScriptableObject
{
    public string blueprintName;
    public Sprite icon;
    public string ID;
    public string description;

    public GameObject projectilePrefab = null;
    public List<ComponentSlot> componentSlots = new();

}
