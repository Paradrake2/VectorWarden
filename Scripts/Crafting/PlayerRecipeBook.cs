using UnityEngine;
using System.Collections.Generic;

public class PlayerRecipeBook : MonoBehaviour
{
    public static PlayerRecipeBook Instance;
    public List<CraftingRecipe> knownRecipes = new();
    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LearnRecipe(CraftingRecipe recipe) {
        if (!knownRecipes.Contains(recipe)) {
            knownRecipes.Add(recipe);
        }
    }

    public bool HasRecipe(CraftingRecipe recipe) {
        return knownRecipes.Contains(recipe);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
