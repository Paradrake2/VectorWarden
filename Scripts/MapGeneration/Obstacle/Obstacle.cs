using System.Collections.Generic;
using UnityEngine;


public class Obstacle : MonoBehaviour
{
    public float obstacleHealth;
    public float maxObstacleHealth;
    public Sprite[] sprites; // Damage progression
    public MapGenerator mapGenerator;
    public int x;
    public int y;
    public List<Items> resourceDrops;
    public float dropChance = 0.5f;
    void Start()
    {

    }
    public void ObstacleDamage(float amount)
    {
        obstacleHealth -= amount;
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (sprites.Length > 0)
        {
            int index = Mathf.Clamp(Mathf.FloorToInt((1 - (obstacleHealth / maxObstacleHealth)) * sprites.Length), 0, sprites.Length - 1);
            GetComponent<SpriteRenderer>().sprite = sprites[index];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (obstacleHealth <= 0)
        {
            DestroyObstacle();
        }
    }
    void DestroyObstacle()
    {
        if (mapGenerator != null) mapGenerator.MakeWalkable(x, y);
        if (resourceDrops != null && resourceDrops.Count > 0 && Random.value < dropChance) AddResource();
        Destroy(gameObject);
    }
    void AddResource()
    {
        Items resource = resourceDrops[Random.Range(0, resourceDrops.Count)];
        int amount = Random.Range(1, 5);
        //InventorySystem.Instance.AddItem(resource.name, amount);
    }
}
