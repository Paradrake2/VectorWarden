using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public BaseMovement movement;
    public EnemyStats stats;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        movement.UpdateMovement();
    }
    public void Die()
    {
        List<Items> drops = stats.getDrop();
        foreach (var item in drops)
        {
            InventorySystem.Instance.AddItem(item.ID, 1);
        }
        Destroy(gameObject);
    }
}
