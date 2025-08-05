using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;
    public UIManager uiManager;
    public XPUIManager xpuiManager;
    public MapGenerator mapGenerator;
    public GameObject playerPrefab;
    public int totalEnemies = 0;
    public int enemiesKilled = 0;
    public int floor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Dungeon")
        {
            GameObject player = Instantiate(playerPrefab, mapGenerator.playerSpawnPosition, Quaternion.identity);
            Camera.main.GetComponent<CameraFollow>().target = player.transform;
            mapGenerator.GenerateRoom();
            uiManager.UpdateHealthText();
            uiManager.UpdateShieldText();
            xpuiManager.UpdateXPText();
            xpuiManager.UpdateXPBarFill();
            xpuiManager.UpdateLevelText();
        }
    }
    public int getFloor()
    {
        return floor;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
