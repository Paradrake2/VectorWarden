using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EquipmentSlotManager : MonoBehaviour
{
    public EquipmentSlot slotType;
    public Image iconImage;
    //public TextMeshProUGUI text;
    public bool isAccessory = false;
    public int accessoryIndex;
    public void SetItem(Equipment item) {
        if (item!= null && item.icon != null) {
            try {
                iconImage.sprite = item.icon;
                iconImage.enabled = true;
            } catch (NullReferenceException e) {
                Debug.LogError(e.StackTrace);
            }
        } else {
            //iconImage.sprite = null;
            //iconImage.enabled = false;
        }
    }
}
