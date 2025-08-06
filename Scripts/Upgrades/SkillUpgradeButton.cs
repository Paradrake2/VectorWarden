using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SkillUpgradeButton : MonoBehaviour
{
    public CardUnlockNode cardNode;
    public Image shader;
    public UpgradeManager upgradeManager;
    void Start()
    {
        if (cardNode == null)
        {
            Debug.LogError("CardUnlockNode is not assigned in SkillUpgradeButton.");
            return;
        }
        UpdateShader();
        if (upgradeManager == null)
        {
            upgradeManager = UpgradeManager.Instance;
            if (upgradeManager == null)
            {
                Debug.LogError("UpgradeManager instance not found.");
            }
        }
    }
    public void DisplayRequirements()
    {
        string requirementsText = "Requirements:\n";
        foreach (var requirement in cardNode.requirements)
        {
            requirementsText += $"{requirement.Key.itemName}: {requirement.Value}\n";
        }
        Debug.Log(requirementsText);
    }
    public void SelectCardNode()
    {
        if (HasRequiredItems() && HasRequiredXP() && HasPrerequisites())
        {
            // Show card selection UI
            CardUnlockUIManager.Instance.BringUpConfirmPanel(cardNode);
        }
        else
        {
            CardUnlockUIManager.Instance.ShowRequirementsPanel(cardNode);
        }
    }
    bool HasRequiredItems()
    {
        foreach (var requirement in cardNode.requirements)
        {
            if (InventorySystem.Instance.itemStacks.Find(i => i.itemId == requirement.Key.ID)?.quantity < requirement.Value)
            {
                return false; // Not enough of the required item
            }
        }
        return true; // All requirements met
    }
    bool HasRequiredXP()
    {
        return upgradeManager.GetUpgradeXPAmount() >= cardNode.requiredXP;
    }
    bool HasPrerequisites()
    {
        foreach (var prerequisite in cardNode.prerequisites)
        {
            if (!prerequisite.isUnlocked)
            {
                return false; // Prerequisite not met
            }
        }
        return true; // All prerequisites met
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void UpdateShader()
    {
        if (HasRequiredItems() && HasRequiredXP() && HasPrerequisites())
        {
            shader.gameObject.SetActive(false);
        }
        else
        {
            shader.gameObject.SetActive(true);
        }
    }
    /*
    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
    */
}
