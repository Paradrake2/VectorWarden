using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUIManager : MonoBehaviour
{
    public static UpgradeUIManager Instance;
    public PlayerStats playerStats;

    public Transform statUpgradePanel;

    [Header("Not Enough XP panel")]
    public GameObject notEnoughXPPanel;
    public TextMeshProUGUI notEnoughXPText;
    [Header("Not Enough Items Panel")]
    public GameObject notEnoughItemsPanel;
    public TextMeshProUGUI notEnoughItemsText;
    
    public Button equipmentButton;
    public Button startMenuButton;
    public List<GameObject> statUpgradeButtons;
    public TextMeshProUGUI statsText;

    public Transform GoldConversionPanel;
    public GoldConversionUIManager goldConversionUIManager;
    void Start()
    {
        playerStats = PlayerStats.Instance;
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats instance not found.");
        }
        notEnoughXPPanel.SetActive(false);
        GoldConversionPanel.gameObject.SetActive(false);
        Instance = this;
    }
    public void ShowNotEnoughXPPanel(float neededXP)
    {
        if (notEnoughXPPanel == null)
        {
            Debug.LogError("NotEnoughXPPanel is not assigned.");
            return;
        }
        StartCoroutine(ShowNotEnoughXPPanelCoroutine(neededXP));
    }
    IEnumerator ShowNotEnoughXPPanelCoroutine(float neededXP)
    {
        notEnoughXPPanel.SetActive(true);
        notEnoughXPText.text = $"Not enough XP to upgrade. Needed xp: {neededXP}";
        yield return new WaitForSeconds(2f);
        notEnoughXPPanel.SetActive(false);
    }
    public void ShowNotEnoughItemsPanel(Items item, int neededAmount)
    {
        if (notEnoughItemsPanel == null)
        {
            Debug.LogError("NotEnoughItemsPanel is not assigned.");
            return;
        }
        StartCoroutine(ShowNotEnoughItemsPanelCoroutine(item, neededAmount));
    }
    IEnumerator ShowNotEnoughItemsPanelCoroutine(Items item, int neededAmount)
    {
        notEnoughItemsPanel.SetActive(true);
        notEnoughItemsText.text = $"Not enough {item.name} to upgrade. Needed amount: {neededAmount}";
        yield return new WaitForSeconds(2f);
        notEnoughItemsPanel.SetActive(false);
    }

    public void UpdateStatsText()
    {
        if (statsText == null)
        {
            Debug.LogError("StatsText is not assigned.");
            return;
        }

        statsText.text = $"Current Stats:\n" +
                        $"Health: {playerStats.BaseHealth}\n" +
                        $"Damage: {playerStats.CurrentDamage}\n" +
                        $"Defense: {playerStats.CurrentDefense}\n" +
                        $"Shield: {playerStats.CurrentShield}\n" +
                        $"XP: {playerStats.CurrentXPGain} \n" +
                        $"Card Options: {playerStats.BaseCardOptions + playerStats.BonusCardOptions} \n";
    }

    public void ShowGoldConversionPanel()
    {
        GoldConversionPanel.gameObject.SetActive(true);
        goldConversionUIManager.PopulateInventory();
    }
    public void HideGoldConversionPanel()
    {
        GoldConversionPanel.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
