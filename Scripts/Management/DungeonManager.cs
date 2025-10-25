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
    private bool yes = true;

    private void Awake()
    {

        Instance = this;


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
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Dungeon")
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
            if (yes)
            {
                Debug.Log("Resetting projectile levels at start of dungeon.");
                ProjectileLevelTracker.Instance.ResetLevels();
            }
            PlayerStats.Instance.ReloadAutoAttackList();
            OrbitalAttack.Instance.ClearOrbitals();
            enemiesKilled = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
