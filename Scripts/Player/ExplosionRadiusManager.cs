using UnityEngine;

public class ExplosionRadiusManager : MonoBehaviour
{
    public static ExplosionRadiusManager Instance;
    public GameObject explosionRadiusIndicatorPrefab;


    public void SpawnRadiusIndicator(Vector3 position, float radius)
    {
        GameObject indicator = Instantiate(explosionRadiusIndicatorPrefab, position, Quaternion.identity);
        indicator.transform.localScale = new Vector3(radius * 2, radius * 2, 1); // Scale to diameter
        indicator.GetComponent<Animator>().SetBool("explosion", true);
        Destroy(indicator, 0.16f); // Destroy after 0.16 seconds
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
