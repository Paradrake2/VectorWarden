using System.Collections;
using System.Linq;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject SaveSuccessful;
    public GameObject LoadSuccessful;
    public GameObject CannotSaveObject;

    void Start()
    {

    }

    public void PlayGame()
    {
        GameManager.Instance.hasOpenedScene = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon");
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT");
    }
    public void SaveData()
    {
        if (GameManager.Instance.hasOpenedScene)
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
                musicVolume = MusicManager.Instance.masterVolume,
                currentUpgradeXP = UpgradeManager.Instance.UpgradeXPAmount,
                cardOptions = PlayerStats.Instance.BonusCardOptions
            };
            DataManager.Save(data);
            StartCoroutine(ShowSaveMessage());
            Debug.Log("Game Saved");
        }
        else
        {
            StartCoroutine(CannotSave());
        }
    }
    IEnumerator CannotSave()
    {
        CannotSaveObject.SetActive(true);
        yield return new WaitForSeconds(2);
        CannotSaveObject.SetActive(false);
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
        GameManager.Instance.hasOpenedScene = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Upgrade");
    }
}
