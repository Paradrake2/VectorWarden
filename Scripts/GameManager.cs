using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public EnemySpawn enemySpawn;
    public TextMeshProUGUI timerText;
    public int maxEnemies = 100;
    public int enemyLevel = 1;
    private float elapsedTime = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadDungeon()
    {
        SceneManager.LoadScene("Dungeon");
    }
    void Update()
    {
        if (timerText != null)
        {
            UpdateTimer();
        }
    }
    private void UpdateTimer()
    {
        elapsedTime += Time.deltaTime;
        int roundedTime = Mathf.CeilToInt(elapsedTime);
        timerText.text = "Time: " + roundedTime.ToString();
    }
    public void SpawnEnemy(Vector3 position)
    {
        //enemySpawn.SpawnEnemy(position);
    }
    public void IncreaseEnemyLevel()
    {
        enemyLevel++;
        maxEnemies += 5;
    }
}
