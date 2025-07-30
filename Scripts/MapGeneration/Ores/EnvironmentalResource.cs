using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalResource : MonoBehaviour
{
    public List<Items> resourceDrops;
    void CollectResource() {
        Items resource = resourceDrops[Random.Range(0,resourceDrops.Count)];
        int amount = Random.Range(1,5);
        InventorySystem.Instance.AddItem(resource.name, amount);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Player") {
            Debug.LogWarning("Hit resource");
            CollectResource();
            Destroy(gameObject);
        }
    }
}
