using System.Collections;
using System.Linq;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject SaveSuccessful;
    public GameObject LoadSuccessful;
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
            gold = PlayerStats.Instance.goldAmount,
            xp = PlayerStats.Instance.CurrentXPGain,
            unlockedCardNodes = UnlockState.Instance.unlockedNodes.ToList(),
            inventoryItems = InventorySystem.Instance.itemStacks.ToList(),
        };
        DataManager.Save(data);
        StartCoroutine(ShowSaveMessage());
        Debug.Log("Game Saved");
    }
    IEnumerator ShowSaveMessage()
    {
        SaveSuccessful.SetActive(true);
        yield return new WaitForSeconds(2);
        SaveSuccessful.SetActive(false);
    }
    IEnumerator ShowLoadMessage()
    {
        LoadSuccessful.SetActive(true);
        yield return new WaitForSeconds(2);
        LoadSuccessful.SetActive(false);
    }
    public void LoadData()
    {
        DataManager.Load();
        StartCoroutine(ShowLoadMessage());
    }

    public void LoadUpgradeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Upgrade");
    }
}
