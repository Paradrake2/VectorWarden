[System.Serializable]
public class InventoryItem
{
    public string itemId;
    public int quantity;

    public int getQuantity() {
        return quantity;
    }

    public string getItemId() {
        return itemId;
    }

}
