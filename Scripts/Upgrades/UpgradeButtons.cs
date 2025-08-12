using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeButtons : MonoBehaviour
{
    public void LaunchRun()
    {
        PlayerStats.Instance.ClearTempStats();
        SceneManager.LoadScene("Dungeon");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
