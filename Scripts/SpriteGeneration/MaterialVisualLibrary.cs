using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialVisualLibrary : MonoBehaviour
{
    public static MaterialVisualLibrary Instance;

    public List<MaterialVisualData> materials;
    private Dictionary<String, MaterialVisualData> materialLookup;
    void Awake()
    {
        Instance = this;
        materialLookup = materials.ToDictionary(m => m.materialId);
    }
    public MaterialVisualData GetVisualData(string materialId) {
        return materialLookup.TryGetValue(materialId, out var data) ? data : null;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
