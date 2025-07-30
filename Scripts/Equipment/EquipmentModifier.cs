using UnityEngine;

public static class EquipmentModifier
{
    public static Equipment eq;
    public static void AddXP(float xp, Equipment equipment) {
        equipment.equipmentXP += xp;
        if (equipment.equipmentXP >= equipment.equipmentLevel*100) {
            equipment.equipmentXP = 0;
            equipment.equipmentLevel++;
            EquipmentLevelUp(equipment);
        }
    }
    public static int XpNeeded(Equipment equipment) {
        return equipment.equipmentLevel * 2000;
    }
    public static void EnhanceWithPowder(float powderXP, Equipment equipment) {
        AddXP(powderXP, equipment);
    }

    public static void EquipmentLevelUp(Equipment equipment) {
        foreach (var mod in equipment.modifiers) {
            if (mod.flatAmount > 0f || mod.percentAmount > 0f) {
                mod.percentAmount += 0.01f;
                Debug.Log($"Boosted {mod.statType} + 1% on {equipment.itemName}");
            }
        }
    }
}
