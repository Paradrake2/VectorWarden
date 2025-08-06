using UnityEngine;
using UnityEngine.UI;

public class SkillIconUIManager : MonoBehaviour
{
    public Transform skillIconContainer; // The container for skill icons
    public GameObject skillIconPrefab; // The prefab for skill icons
    public PlayerStats playerStats;
    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindFirstObjectByType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogError("PlayerStats not found in the scene.");
                return;
            }
        }
        UpdateSkillIcons();
    }

    public void UpdateSkillIcons()
    {
        // Clear existing icons
        foreach (Transform child in skillIconContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new icons based on active skill cards
        foreach (var skillCard in playerStats.activeSkillCards)
        {
            Sprite icon = skillCard.skillIcon;
            GameObject iconObject = Instantiate(skillIconPrefab, skillIconContainer);
            iconObject.GetComponent<Image>().sprite = icon;
            iconObject.GetComponent<Image>().enabled = true;
            iconObject.GetComponent<SkillIcon>().enabled = true;
        }
    }

    void Update()
    {
        
    }
}
