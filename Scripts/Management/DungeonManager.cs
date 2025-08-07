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
    public int floormasterThreshold = 30;
    public int floor;
    public bool floormasterSpawned = false;

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
            floormasterSpawned = false;
            GameObject player = Instantiate(playerPrefab, mapGenerator.playerSpawnPosition, Quaternion.identity);
            Camera.main.GetComponent<CameraFollow>().target = player.transform;
            mapGenerator.GenerateRoom();
            uiManager.UpdateHealthText();
            uiManager.UpdateShieldText();
            uiManager.UpdateDamageText();
            uiManager.UpdateDefenseText();
            xpuiManager.UpdateXPText();
            xpuiManager.UpdateXPBarFill();
            xpuiManager.UpdateLevelText();
        }
    }
    public int getFloor()
    {
        return floor;
    }
    public void EnemyKilled()
    {
        enemiesKilled++;
        if (enemiesKilled >= floormasterThreshold && !floormasterSpawned)
        {
            Floormaster.Instance.SpawnFloormaster();
            floormasterSpawned = true;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
