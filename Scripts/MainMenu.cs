using System.Linq;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {

    }

    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon");
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT");
    }
    public void SaveData()
    {
        SaveData data = new SaveData
        {
            maxHealth = PlayerStats.Instance.CurrentMaxHealth,
            damage = PlayerStats.Instance.CurrentDamage,
            defense = PlayerStats.Instance.CurrentDefense,
            unlockedCards = UnlockState.Instance.unlockedNodes.ToList(),
            inventoryItems = InventorySystem.Instance.itemStacks.ToList(),
        };
        DataManager.Save(data);
        Debug.Log("Game Saved");
    }
    public void LoadData()
    {
        DataManager.Load();
    }

    public void LoadUpgradeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Upgrade");
    }
}
