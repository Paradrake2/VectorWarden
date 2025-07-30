using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowderMaker : MonoBehaviour
{
    public static PowderMaker Instance;
    public Transform coreListParent;
    public GameObject holder;
    public GameObject coreButtonPrefab;
    public GameObject powderTextAmount;
    public List<Items> coreList;


    List<Items> GenerateCoreList() {
        List<Items> cores = new List<Items>();
        foreach(var stack in InventorySystem.Instance.itemStacks) {
            Items item = ItemRegistry.Instance.GetItemById(stack.itemId);
            if(item != null && item.tags != null && System.Array.Exists(item.tags, tag => tag == "core")) {
                cores.Add(item);
            }
        }
        return cores;
    }
    public void PopulateCoreInventory() {
        foreach (Transform child in coreListParent) {
            Destroy(child.gameObject);
        }
        foreach(var stack in InventorySystem.Instance.itemStacks) {
            Items item = ItemRegistry.Instance.GetItemById(stack.itemId);
            if (item != null && item.tags != null && System.Array.Exists(item.tags, t => t == "core")) {
                var btn = Instantiate(coreButtonPrefab, coreListParent);
                var icon = btn.transform.Find("Icon")?.GetComponent<UnityEngine.UI.Image>();
                var text = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>();

                if(icon != null) icon.sprite = item.icon;
                if(text != null) text.text = $"{item.itemName} x {stack.quantity}";

                btn.GetComponent<Button>().onClick.AddListener(() => {
                    PowderInventory.Instance.CoreToPowder(item);
                    PowderAmount();
                    PopulateCoreInventory();
                });
            }
        }
    }
    public void PowderAmount()
    {
        TextMeshProUGUI txt = powderTextAmount.GetComponent<TextMeshProUGUI>();
        txt.text =$"Powder total: {PowderInventory.Instance.totalPowder}";
    }
    public void OpenCoreRefineMenu()
    {
        holder.SetActive(true);
        PopulateCoreInventory();
    }
    public void CloseCoreRefineMenu() {
        holder.SetActive(false);
        CraftingUIManager.Instance.PopulateInventory(0);
    }
    void Start()
    {
        
    }
}
