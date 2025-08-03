using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RefineRecipe", menuName = "Scriptable Objects/RefineRecipe")]
public class RefineRecipe : ScriptableObject
{
    public string recipeName;
    public Sprite icon;
    public float tier;

    public List<string> validMaterialTags;
}
