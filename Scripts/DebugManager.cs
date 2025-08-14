using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }


    public Transform debugPanel;

    void Start()
    {
        Instance = this;
        debugPanel.gameObject.SetActive(false);
    }
    public void ReloadAutoFire()
    {
        PlayerStats.Instance.ReloadAutoAttackList();
    }
    public void ToggleDebugPanel()
    {
        if (debugPanel != null)
        {
            debugPanel.gameObject.SetActive(!debugPanel.gameObject.activeSelf);
        }
        else
        {
            Debug.LogError("Debug panel is not assigned.");
        }
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
