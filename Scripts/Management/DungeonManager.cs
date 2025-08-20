using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;
    public UIManager uiManager;
    public XPUIManager xpuiManager;
    public MapGenerator mapGenerator;
    public GameObject playerPrefab;
    public int enemiesKilled = 0;
    public int floormasterThreshold = 30;
    public TextMeshProUGUI killCountText;
    public int floor;

    private void Awake()
    {

        Instance = this;


    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Dungeon")
        {
            
            GameObject player = Instantiate(playerPrefab, mapGenerator.playerSpawnPosition, Quaternion.identity);
            Camera.main.GetComponent<CameraFollow>().target = player.transform;
            UIManager.Instance?.BindPlayer(player.GetComponent<Player>());
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
        killCountText.text = "Enemies Killed: " + enemiesKilled;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
