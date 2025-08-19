using System.Linq;
using TMPro;
using UnityEngine;

public class CardUnlockUIManager : MonoBehaviour
{
    public static CardUnlockUIManager Instance;
    [Header("Unlocked Cards Panel")]
    public GameObject unlockedCardsPanel;
    [Header("Confirm Panel")]
    public GameObject confirmPanel;
    public TextMeshProUGUI confirmText;
    public TextMeshProUGUI confirmNodeName;
    public TextMeshProUGUI confirmNodeRequirements;
    public TextMeshProUGUI confirmNodeXP;
    [Header("Requirements panel")]
    public GameObject requirementsPanel;
    public TextMeshProUGUI requirementsText;

    [SerializeField] private CardUnlockNode node = null;
    void Start()
    {
        Instance = this;
        confirmPanel.SetActive(false);
        requirementsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // this is what the confirm button will do
    public void UnlockCards()
    {
        UnlockSkillCards(node);
        DisableSkillCards(node);
        UnlockState.Instance.NodeUnlock(node);
        RemoveRequirements();
        UpgradeMaterialInventoryManager.Instance.PopulateInventory();
        UpdateSkillUpgradeButtons();
        node = null;
        ClearConfirmPanel();
    }
    public void UnlockSkillCards(CardUnlockNode cardNode)
    {
        foreach (var card in cardNode.unlockedCards)
        {
            UnlockState.Instance.UnlockCard(card);
        }
    }
    public void DisableSkillCards(CardUnlockNode cardNode)
    {
        foreach (var card in cardNode.disabledCards)
        {
            card.unlocked = false;
        }
    }
    public void SetNode(CardUnlockNode cardNode)
    {
        node = cardNode;
        if (node == null)
        {
            Debug.LogError("CardUnlockNode is not set.");
            return;
        }
        // Update the UI with the node's information
        // For example, you can update a text field or an image to show the card's name and icon
        Debug.Log($"Selected Card Node: {node.unlockID}");
    }
    public void BringUpConfirmPanel(CardUnlockNode node)
    {
        confirmPanel.SetActive(true);
        confirmText.text = $"Are you sure you want to unlock {node.unlockID}?";
        confirmNodeName.text = node.unlockID;
        confirmNodeRequirements.text = string.Join(", ", node.requirements.Select(r => r.item.itemName));
        confirmNodeXP.text = node.requiredXP.ToString();
        SetNode(node);
    }
    public void ClearConfirmPanel()
    {
        confirmPanel.SetActive(false);
        confirmText.text = string.Empty;
        confirmNodeName.text = string.Empty;
        confirmNodeRequirements.text = string.Empty;
        confirmNodeXP.text = string.Empty;
        node = null;
    }
    public void RemoveRequirements()
    {
        foreach (var requirement in node.requirements)
        {
            InventorySystem.Instance.RemoveItem(requirement.item.ID, requirement.amount);
        }
        UpgradeManager.Instance.RemoveXP(node.requiredXP);
    }
    public void ShowRequirementsPanel(CardUnlockNode node)
    {
        requirementsPanel.SetActive(true);
        requirementsText.text = $"Requirements for {node.unlockID}:\n";
        foreach (var requirement in node.requirements)
        {
            requirementsText.text += $"{requirement.item.itemName}: {requirement.amount}\n";
        }
        requirementsText.text += $"Required XP: {node.requiredXP}";
        requirementsText.text += $"\nPrerequisites: {string.Join(", ", node.prerequisites.Select(p => p.unlockID))}";
    }
    public void HideRequirementsPanel()
    {
        requirementsPanel.SetActive(false);
    }
    private void UpdateSkillUpgradeButtons()
    {
        SkillUpgradeButton[] buttons = FindObjectsByType<SkillUpgradeButton>(FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            button.UpdateShader();
        }
    }
}
