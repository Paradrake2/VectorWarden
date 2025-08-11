using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class FloormasterSpawner
{
    public int minFloor; // Minimum floor in the range
    public int maxFloor; // Maximum floor in the range
    public List<GameObject> floormasters;
}

public class Floormaster : MonoBehaviour
{
    public static Floormaster Instance;
    public DungeonManager dungeonManager;
    public List<FloormasterSpawner> floormasters;
    public TextMeshProUGUI floormasterTimerText;

    private float spawnTimer = 20f;
    private float pauseDuration = 30f;
    private bool isFloormasterAlive = false;
    private GameObject currentFloormaster;
    private Coroutine floormasterCoroutine;

    void Start()
    {
        dungeonManager = FindFirstObjectByType<DungeonManager>();
        if (dungeonManager == null)
        {
            Debug.LogError("DungeonManager not found in the scene.");
            return;
        }

        // Start the Floormaster timer coroutine
        floormasterCoroutine = StartCoroutine(FloormasterTimer());
    }

    void OnDisable()
    {
        // Stop the coroutine when the object is disabled
        if (floormasterCoroutine != null)
        {
            StopCoroutine(floormasterCoroutine);
        }
    }

    void OnDestroy()
    {
        // Stop the coroutine when the object is destroyed
        if (floormasterCoroutine != null)
        {
            StopCoroutine(floormasterCoroutine);
        }
    }

    public void SpawnFloormaster()
    {
        int floor = DungeonManager.Instance.floor;
        FloormasterSpawner spawner = floormasters.FirstOrDefault(s => floor >= s.minFloor && floor <= s.maxFloor);
        if (spawner != null)
        {
            GameObject floormaster = spawner.floormasters[Random.Range(0, spawner.floormasters.Count)];
            currentFloormaster = Instantiate(floormaster, GetSpawnPosition(), Quaternion.identity);
            isFloormasterAlive = true;
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 playerPosition = FindFirstObjectByType<PlayerStats>().transform.position;
        float angle = Random.Range(0f, Mathf.PI * 2); // Get random direction on unit circle
        Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;
        float distance = Random.Range(15f, 30f);
        return playerPosition + direction * distance;
    }

    IEnumerator FloormasterTimer()
    {
        while (true)
        {
            while (spawnTimer > 0 && !isFloormasterAlive)
            {
                spawnTimer -= Time.deltaTime;
                yield return null;
            }
            if (spawnTimer <= 0 && !isFloormasterAlive)
            {
                SpawnFloormaster();
                floormasterTimerText.text = "Floormaster Active!";
                float pauseTime = pauseDuration;
                while (pauseTime > 0 && currentFloormaster != null)
                {
                    pauseTime -= Time.deltaTime;
                    yield return null;
                }

                isFloormasterAlive = false;
                spawnTimer = 60f;
            }
            yield return null;
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Dungeon")
        {
            return;
        }
        if (!isFloormasterAlive)
        {
            floormasterTimerText.text = "Floormaster Timer: " + Mathf.CeilToInt(spawnTimer).ToString();
        }
        else
        {
            floormasterTimerText.text = "Floormaster Active!";
        }
    }
}
