using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RefineRecipe", menuName = "Scriptable Objects/RefineRecipe")]
public class RefineRecipe : ScriptableObject
{
    public string recipeName;
    public float tier;
    public Sprite icon;
    public int outputQuantity = 1;
    [Header("Ingredient tags")]
    public List<RecipeSlotRequirement> requirements = new();
    public Items outputItem;
    public string[] GetTags()
    {
        foreach (var tag in requirements) return requirements.Select(r => r.requiredTag).Distinct().ToArray();
        return null;
    }
}
