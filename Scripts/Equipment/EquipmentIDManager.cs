using System.Collections.Generic;
using UnityEngine;

public static class EquipmentIDManager
{
    private static HashSet<string> existingIDs = new HashSet<string>();

    public static bool IsIDTaken(string id)
    {
        return existingIDs.Contains(id);
    }

    public static void RegisterID(string id)
    {
        existingIDs.Add(id);
    }
}
