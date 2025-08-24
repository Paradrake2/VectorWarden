using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Items item;
    public AudioSource pickupSound;
    public float volume = 1f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ItemPickup"))
        {
            Debug.Log("Picked up " + item.itemName);
            InventorySystem.Instance.AddItemToSpoils(item.ID, 1);
            SFXManager.Instance.PlayPickupSound(pickupSound.clip, volume);
            Destroy(gameObject);
        }
    }
}
