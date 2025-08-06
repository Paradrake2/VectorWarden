using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    public Image icon;
    public void SetIcon(Sprite newIcon)
    {
        icon.sprite = newIcon;
    }
}
