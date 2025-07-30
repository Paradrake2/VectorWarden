using UnityEngine;
using TMPro;

public class HoverNameUI : MonoBehaviour
{
    public static HoverNameUI Instance;
    public TextMeshProUGUI hoverText;
    public RectTransform hoverRect;
    public Vector2 offset = new Vector2(15, -15);
    public Canvas canvas;

    private bool isHovering = false;
    private string currentText = "";

    void Awake()
    {
        hoverText.text = "";
        hoverText.gameObject.SetActive(false);
        canvas = GetComponentInParent<Canvas>();
            Instance = this;
    }

    public void Show(string name)
    {
        if (isHovering && currentText == name)
            return; // Already showing the same tooltip, skip redundant SetActive

        currentText = name;
        isHovering = true;
        hoverText.text = name;
        hoverText.gameObject.SetActive(true);
    }

    public void Hide()
    {
        isHovering = false;
        currentText = "";
        hoverText.text = "";
        hoverText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isHovering)
        {
            Vector2 mousePos = Input.mousePosition;
            hoverRect.position = mousePos + offset;
        }
    }
}
