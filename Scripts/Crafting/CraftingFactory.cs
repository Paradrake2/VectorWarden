using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CraftingFactory : MonoBehaviour
{
    public Equipment GenerateFromIngredients(List<Items> ingredients, CraftingRecipe recipeId) {
        var equipment = new Equipment();
        equipment.itemName = GenerateName(ingredients, recipeId);
        equipment.slot = recipeId.slot;
        equipment.modifiers = new List<StatModifier>();

        recipeId.spriteGenerator = FindFirstObjectByType<SpriteGenerator>();
        Sprite icon = recipeId.spriteGenerator.GenerateIcon(recipeId.visualPrefab, ingredients);

        // Create ID
        equipment.ID = GenerateID();
        if (equipment.ID == null)
        {
            Debug.LogError("Failed Generating Equipment. Reason: Couldn't create unique ID.");
            return null;
        }

        equipment.icon = icon;
        equipment.augmentSlotNumber = recipeId.augmentSlots;
        equipment.allowedAugments = recipeId.augmentSlots;
        equipment.weaponType = recipeId.weaponType;
        if (equipment.weaponType != WeaponType.None)
        {
            equipment.projectilePrefab = recipeId.projectilePrefab;
            equipment.attackRange = recipeId.attackRange;
            equipment.attackSpeed = recipeId.attackSpeed;
            equipment.manaCost = recipeId.manaCost;
            equipment.projectileSpeed = recipeId.projectileSpeed;
        }
        foreach (Items item in ingredients)
        {
            TryAddModifier(equipment, StatType.Damage, item.flatDamage, item.damageMult);
            TryAddModifier(equipment, StatType.Defense, item.flatDefense, item.defenseMult);
            TryAddModifier(equipment, StatType.MaxHealth, item.flatMaxHP, item.HPMult);
            TryAddModifier(equipment, StatType.Mana, item.flatMaxMana, item.maxManaMult);
            TryAddModifier(equipment, StatType.PureDamage, item.flatPureDamage, item.pureDamageMult);
            TryAddModifier(equipment, StatType.XPGain, item.xpGainIncrease, 0);
            TryAddModifier(equipment, StatType.GoldGain, item.goldGainIncrease, 0);
            TryAddModifier(equipment, StatType.DropRate, item.dropRateIncrease, 0);
            TryAddModifier(equipment, StatType.Knockback, item.flatKnockbackIncrease, item.knockbackMult);
            TryAddModifier(equipment, StatType.AttackSpeed, item.attackSpeedFlat, item.attackSpeedMult);
        }
        if (ingredients.Count != recipeId.requirements.Sum(r => r.quantityRequired)) {
            Debug.LogWarning("Invalid number of ingredients");
            return null;
        }
        return equipment;
    }
    public string GenerateID()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        int idLength = 10;
        string id;
        int maxAttempts = 100;

        do
        {
            id = new string(Enumerable.Repeat(chars, idLength).Select(s => s[UnityEngine.Random.Range(0, s.Length)]).ToArray());
            maxAttempts--;
        }
        while (EquipmentIDManager.IsIDTaken(id) && maxAttempts > 0);

        if (maxAttempts == 0)
        {
            Debug.LogError("Failed to generate unique Equipment ID after 100 attempts");
            return null;
        }

        EquipmentIDManager.RegisterID(id);
        return id;
    }
    public Equipment PreviewCraftedEquipment(List<Items> ingredients, CraftingRecipe recipe) {
        return GenerateFromIngredients(ingredients, recipe);
    }
    private void TryAddModifier(Equipment equipment, StatType stat, float flat, float mult) {
        if (flat == 0f && mult == 0f) return;

        var existing = equipment.modifiers.Find(m => m.statType == stat);
        if (existing != null) {
            existing.flatAmount += flat;
            existing.percentAmount += mult;
        } else {
            equipment.modifiers.Add(new StatModifier {
                statType = stat,
                flatAmount = flat,
                percentAmount = mult
            });
        }
    }

    private string GenerateName(List<Items> ingredients, CraftingRecipe recipe) {
        
        var baseName = ingredients[0].itemName;
        // if this is throwing an error it means there are no ingredients
        return $"{baseName} {recipe.getRecipeName()}";
    }
}
