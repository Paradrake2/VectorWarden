using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;
using System;
public class LevelUp : MonoBehaviour
{
    public PlayerStats playerStats;
    public PlayerAttack playerAttack;
    public SkillCardUI skillCardUIPrefab;
    public GameObject levelUpPanel;
    public SkillIconUIManager skillIconUIManager;

    public TextMeshProUGUI cardStatsText;

    public int guaranteedRarityEveryYLevels = 3;
    public int baseCardCount = 3;

    public LevelUp Instance;
    private bool placeholder = true; // this will be used later, maybe. Currently can be used to prevent all projectile cards from being destroyed when the player selects a new projectile card
    public void LevelUpPlayer(int playerLevel, int cardOptions)
    {
        Time.timeScale = 0;
        LevelUpStats(); // Update player stats based on level up
        XPUIManager.Instance.UpdateLevelText();
        levelUpPanel.SetActive(true);
        cardStatsText.gameObject.SetActive(true);
        List<SkillCard> availableCards = GetCardOptions(playerLevel, cardOptions);

        // Clear previous cards
        foreach (Transform child in levelUpPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Wait one frame before adding new cards
        StartCoroutine(ShowSkillCardsNextFrame(availableCards));
    }

    private IEnumerator ShowSkillCardsNextFrame(List<SkillCard> availableCards)
    {
        yield return null; // Wait for end of frame
        foreach (var card in availableCards)
        {
            Debug.Log($"Available Card: {card.skillName} (Rarity: {card.skillCardRarity})");
        }
        foreach (SkillCard skillCard in availableCards)
        {
            SkillCardUI skillCardUI = Instantiate(skillCardUIPrefab, levelUpPanel.transform);
            skillCardUI.skillCard = skillCard;
            skillCardUI.UpdateUI();
        }

        LayoutGroup layout = levelUpPanel.GetComponent<LayoutGroup>();
        if (layout != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
        }
    }

    List<SkillCard> GetCardOptions(int playerLevel, int cardOptions)
    {
        SkillCard[] allCards = Resources.LoadAll<SkillCard>("SkillCards");
        List<SkillCard> filteredByLevel = new List<SkillCard>();
        foreach (var card in allCards)
        {
            if (playerLevel >= card.minimumLevelRequired && card.unlocked)
            {
                filteredByLevel.Add(card);
            }
        }

        if (playerLevel % 5 == 0)
        {
            filteredByLevel = filteredByLevel.FindAll(card => card.skillType == SkillType.Projectile);
        }
        filteredByLevel.RemoveAll(card =>
            card.skillType == SkillType.Projectile &&
            playerStats.activeSkillCards.Any(active => active == card));
        List<SkillCard> cardOptionsList = new List<SkillCard>();
        List<SkillCard> availablePool = new List<SkillCard>(filteredByLevel);
        for (int i = 0; i < cardOptions; i++)
        {
            SkillCard selected = CreateRandomSkillCard(availablePool, playerLevel);
            if (selected != null)
            {
                cardOptionsList.Add(selected);
                availablePool.Remove(selected);
            }
        }
        return cardOptionsList;
    }
    SkillCard CreateRandomSkillCard(List<SkillCard> availableCards, int playerLevel)
    {
        float rand = UnityEngine.Random.value;
        SkillRarity chosenRarity;
        if (rand < 0.002f) chosenRarity = SkillRarity.Legendary;
        else if (rand < 0.1f) chosenRarity = SkillRarity.Epic;
        else if (rand < 0.30f) chosenRarity = SkillRarity.Rare;
        else if (rand < 0.50f) chosenRarity = SkillRarity.Uncommon;
        else chosenRarity = SkillRarity.Common;

        // Filter cards by chosen rarity
        var filteredByRarity = availableCards.FindAll(card => card.skillCardRarity == chosenRarity);

        if (filteredByRarity.Count == 0)
        {
            // If no cards of the chosen rarity, fallback to any available card
            filteredByRarity = availableCards;
        }

        if (filteredByRarity.Count == 0) return null;
        return filteredByRarity[UnityEngine.Random.Range(0, filteredByRarity.Count)];
    }
    public void SelectSkillCard(SkillCard skillCard)
    {
        if (skillCard.skillType == SkillType.Projectile && placeholder)
        {
            playerStats.activeSkillCards.RemoveAll(card => card.skillType == SkillType.Projectile);

        }
        playerStats.activeSkillCards.Add(skillCard);
        UpdatePlayerStats();

        foreach (Transform child in levelUpPanel.transform)
        {
            Destroy(child.gameObject); // Clear the level up panel
        }
        levelUpPanel.SetActive(false);
        cardStatsText.gameObject.SetActive(false);
        skillIconUIManager.UpdateSkillIcons();
        Time.timeScale = 1; // Resume the game
        XPUIManager.Instance.UpdateXPBarFill();
    }
    public void RandomStatBoost(SkillCard card)
    {
        int boostCount = card.skillCardRarity switch
        {
            SkillRarity.Common => 1,
            SkillRarity.Uncommon => 2,
            SkillRarity.Rare => 3,
            SkillRarity.Epic => 4,
            SkillRarity.Legendary => 5,
            _ => 1
        };

        StatType[] possibleStats = new StatType[]
        {
            StatType.MaxHealth,
            StatType.Damage,
            StatType.Defense,
            StatType.PlayerMoveSpeed,
            StatType.AttackCooldown,
            //StatType.ExplosionRadius,
            //StatType.HomingRange,
            //StatType.PierceAmount
        };

        for (int i = 0; i < boostCount; i++)
        {
            StatType chosenStat = possibleStats[UnityEngine.Random.Range(0, possibleStats.Length)];
            float boostValue = UnityEngine.Random.Range(card.minValue, card.maxValue);
            switch (chosenStat)
            {
                case StatType.MaxHealth:
                    playerStats.BaseHealth += (int)boostValue;
                    break;
                case StatType.Damage:
                    playerStats.BaseDamage += (int)boostValue;
                    break;
                case StatType.Defense:
                    playerStats.BaseDefense += (int)boostValue;
                    break;
                case StatType.PlayerMoveSpeed:
                    playerStats.BaseMoveSpeed += boostValue / 100;
                    break;
                case StatType.AttackCooldown:
                    playerStats.BaseAttackCooldown -= boostValue / 70;
                    break;
                    /*
                case StatType.ExplosionRadius:
                    playerStats.BaseExplosionRadius += boostValue / 130;
                    break;
                case StatType.HomingRange:
                    playerStats.BaseHomingRange += boostValue / 100;
                    break;
                case StatType.PierceAmount:
                    int val = Math.Min(3, Math.Max(1, (int)boostValue / 2));
                    playerStats.BasePiercingAmount += val;
                    break;
                    */
            }
        }
        // Show the results
        UpdatePlayerStats();
    }
    void UpdatePlayerStats()
    {
        playerStats.CurrentHealth = playerStats.CurrentMaxHealth;
        playerStats.CurrentMana = playerStats.CurrentMaxMana;
        UIManager.Instance.UpdateShieldText();
        UIManager.Instance.UpdateHealthText();
        UIManager.Instance.UpdateDamageText();
        UIManager.Instance.UpdateDefenseText();
    }
    public void LevelUpStats()
    {
        playerStats.BaseHealth += playerStats.HealthPerLevel;
        playerStats.BaseDamage += playerStats.DamagePerLevel;
        playerStats.BaseDefense += playerStats.DefensePerLevel;
        playerStats.BaseShield += playerStats.ShieldPerLevel;
        
        Debug.LogWarning($"Base shield after level up: {playerStats.BaseShield}");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
