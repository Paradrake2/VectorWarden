using Unity.VisualScripting;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public static OptionsMenu Instance;
    public GameObject optionsMenuPrefab;
    private GameObject _optionsMenuInstance;
    public void PopupOptionsMenu()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (optionsMenuPrefab != null && canvas != null)
        {
            _optionsMenuInstance = Instantiate(this.optionsMenuPrefab, canvas.transform);
            _optionsMenuInstance.transform.SetAsLastSibling(); // Ensure it's on top
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
