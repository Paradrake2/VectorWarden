using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[System.Serializable]
public class RecipeSlotRequirement {
    public string requiredTag;
    public int quantityRequired;
}
public class PartVisual {
    public string partName;
    public Color tintColor;
}
public enum MaterialRoleType {
    Plate,
    Binding,
    Blade,
    Guard,
    Handle,
    Accessory,
    Gem
}
[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Scriptable Objects/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeId;
    [System.NonSerialized] public SpriteGenerator spriteGenerator;
    public string recipeName;
    public EquipmentSlot slot;
    //public int requiredSlots;
    public SlotCategory slotCategory;
    public List<RecipeSlotRequirement> requirements = new();
    public List<EquipmentPartSprite> parts;
    public Sprite icon;
    public Sprite baseImage;
    public MaterialVisualData[] visualLayers;
    public GameObject visualPrefab;
    public int augmentSlots;
    public int requiredLevel;
    public float recipeTier;

    // For weapons
    public WeaponType weaponType;
    public float attackRange = 0f;
    public float attackSpeed = 1f;
    public float manaCost = 0f;
    public float projectileSpeed = 0f;
    public GameObject projectilePrefab;
    public bool isUnlocked = false;
    public List<StatModifier> baseStats = new();
    public string getRecipeId()
    {
        return recipeId;
    }
    public string getRecipeName() {
        return recipeName;
    }

    public EquipmentSlot GetEquipmentSlot() {
        return slot;
    }
    public Sprite getIcon() {
        return icon;
    }

    public List<RecipeSlotRequirement> getRequirements() {
        return requirements;
    }
    public int getAugmentSlots() {
        return augmentSlots;
    }
    public string[] GetTags()
    {
        foreach (var tag in requirements) return requirements.Select(r => r.requiredTag).Distinct().ToArray();
        return null;
    }

    public void GenerateSprite(GameObject visualPrefab, List<Items> ingredients)
    {
        spriteGenerator.GenerateIcon(visualPrefab, ingredients);
    }
}
