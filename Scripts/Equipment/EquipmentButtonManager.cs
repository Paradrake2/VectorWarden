using UnityEngine;
using UnityEngine.SceneManagement;

public class EquipmentButtonManager : MonoBehaviour
{
    public void loadCraftingScene() {
        SceneManager.LoadScene("Menu");
        
    }
    
    public void loadDungeon() {
        PlayerStats.Instance.CurrentHealth = PlayerStats.Instance.CurrentMaxHealth;
        DungeonManager.Instance.floor = 0;
        SceneManager.LoadScene("Dungeon");
    }
    public void loadStats() {
        SceneManager.LoadScene("Stats");
    }
    public void loadAugment() {
        SceneManager.LoadScene("Augment");
    }
    public void loadSkillTree()
    {
        SceneManager.LoadScene("SkillTree");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
