using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeButtons : MonoBehaviour
{
    public void LaunchRun()
    {
        PlayerStats.Instance.ClearTempStats();
        SceneManager.LoadScene("Dungeon");
        UpgradeXPHolder.Instance.SetUpgradeXPAmount(UpgradeManager.Instance.GetUpgradeXPAmount());
    }
    public void BackToMenu()
    {
        PlayerStats.Instance.ClearTempStats();
        SceneManager.LoadScene("StartMenu");
    }
}
