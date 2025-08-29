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
    private int pendingLevelUps = 0;
    private bool isPanelOpen = false;
    public static LevelUp Instance;
    public void BindPlayerStats(PlayerStats stats)
    {
        playerStats = stats;
    }
    void Awake()
    {
        if (playerStats == null) playerStats = PlayerStats.Instance;
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
    Instance = this;
    }
    void OnDestroy() { if (Instance == this) Instance = null; }

    public void LevelUpPlayer(int playerLevel, int cardOptions)
    {
        Time.timeScale = 0;
        Debug.LogWarning("LevelUpPlayer running");
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
    public void EnqueueLevelUps(int count, int playerLevel, int cardOptions)
    {
        pendingLevelUps += count;
        if (!isPanelOpen)
        {
            ShowLevelUpPanel(playerLevel, cardOptions);
        }
    }
    private void ShowLevelUpPanel(int playerLevel, int cardOptions)
    {
        if (pendingLevelUps <= 0) return;
        isPanelOpen = true;
        Time.timeScale = 0;
        levelUpPanel.SetActive(true);
        LevelUpPlayer(playerLevel, cardOptions);
    }
    private IEnumerator ShowSkillCardsNextFrame(List<SkillCard> availableCards)
    {
        yield return null; // Wait for end of frame
        foreach (var card in availableCards)
        {
            Debug.Log($"Available Card: {card.skillName} (Rarity: {card.skillCardRarity})");
        }
        List<Button> cardButtons = new List<Button>();
        foreach (SkillCard skillCard in availableCards)
        {
            SkillCardUI skillCardUI = Instantiate(skillCardUIPrefab, levelUpPanel.transform);
            skillCardUI.skillCard = skillCard;
            skillCardUI.UpdateUI();
            Button cardButton = skillCardUI.GetComponent<Button>();
            cardButton.interactable = false;
            cardButtons.Add(cardButton);
        }

        LayoutGroup layout = levelUpPanel.GetComponent<LayoutGroup>();
        if (layout != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
        }
        yield return new WaitForSecondsRealtime(1);
        foreach (Button cardButton in cardButtons)
        {
            cardButton.interactable = true; // Enable buttons after a short delay
        }
    }

    List<SkillCard> GetCardOptions(int playerLevel, int cardOptions)
    {
        SkillCard[] allCards = Resources.LoadAll<SkillCard>("SkillCards");
        List<SkillCard> filteredByLevel = new List<SkillCard>();
        foreach (var card in allCards)
        {
            if (playerLevel >= card.minimumLevelRequired && (card.unlocked || (UnlockState.Instance != null && UnlockState.Instance.IsCardUnlocked(card))))
            {
                bool prerequisitesMet = card.prerequisites == null || card.prerequisites.Count == 0 || card.prerequisites.Any(prereq => playerStats.activeSkillCards.Contains(prereq));

                // Filter out projectile cards at max level
                bool notMaxProjectile = true;
                if ((card.skillType == SkillType.Projectile || card.skillType == SkillType.AutoAttack)&& card.projectileData != null)
                {
                    var upgrade = ProjectileLevelTracker.Instance.upgradeAssets
                        .FirstOrDefault(u => u.projectileData == card.projectileData);
                    int currentLevel = ProjectileLevelTracker.Instance.GetLevel(card.projectileData);
                    if (upgrade != null && currentLevel >= upgrade.maxLevel - 1)
                    {
                        notMaxProjectile = false;
                    }
                }

                if (prerequisitesMet && notMaxProjectile)
                {
                    filteredByLevel.Add(card);
                }
            }
        }
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
        if (skillCard.skillType == SkillType.Projectile || skillCard.skillType == SkillType.AutoAttack)
        {
            //playerStats.activeSkillCards.RemoveAll(card => card.skillType == SkillType.Projectile);
            if (playerStats.activeSkillCards.Contains(skillCard))
            {
                UpgradeProjectile(skillCard.projectileData);
                
                if (skillCard.projectileData.projectileType == ProjectileType.Orbital)
                {
                    OrbitalAttack.Instance.RefreshOrbitals();
                }
            }
        }
        bool addToCardList = true;
        if (playerStats.activeSkillCards.Contains(skillCard) && (skillCard.skillType == SkillType.Projectile || skillCard.skillType == SkillType.AutoAttack)) addToCardList = false;
        if (addToCardList) playerStats.activeSkillCards.Add(skillCard);
        UpdatePlayerStats();

        foreach (Transform child in levelUpPanel.transform)
        {
            Destroy(child.gameObject); // Clear the level up panel
        }
        
        levelUpPanel.SetActive(false);
        cardStatsText.gameObject.SetActive(false);
        isPanelOpen = false;

        pendingLevelUps--;
        if (pendingLevelUps > 0)
        {
            isPanelOpen = true;
            var nextLevel = playerStats.Level;
            var options = playerStats.GetNumCardOptions();
            ShowLevelUpPanel(nextLevel, options);
        }
        else
        {
            Time.timeScale = 1;
        }


        if (skillCard.autoAttackDataList != null && skillCard.autoAttackDataList.Count > 0)
        {
            foreach (var list in skillCard.autoAttackDataList)
            {
                if (list != null)
                {
                    playerStats.AddAutoAttackToList(list);
                }
            }
        }
        
        playerStats.AddCardToOwnedSkillCards(skillCard); // Adds the card to the list of cards the player has used
        
        skillIconUIManager.UpdateSkillIcons();
        XPUIManager.Instance.UpdateXPBarFill();
    }
    void UpgradeProjectile(ProjectileData projData)
    {
        ProjectileLevelTracker.Instance.AddLevel(projData);
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
                case StatType.MaxHealth: playerStats.AddTempFlat(StatType.MaxHealth, boostValue); break;
                case StatType.Damage: playerStats.AddTempFlat(StatType.Damage, boostValue); break;
                case StatType.Defense: playerStats.AddTempFlat(StatType.Defense, boostValue); break;
                case StatType.PlayerMoveSpeed: playerStats.AddTempPercent(StatType.PlayerMoveSpeed, boostValue / 100f); break;
                case StatType.AttackCooldown: playerStats.AddTempFlat(StatType.AttackCooldown, -boostValue / 70f); break; // matches your old logic

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
        //playerStats.CurrentHealth = playerStats.CurrentMaxHealth;
        //playerStats.CurrentMana = playerStats.CurrentMaxMana;
        UIManager.Instance.UpdateShieldText();
        UIManager.Instance.UpdateHealthText();
        UIManager.Instance.UpdateDamageText();
        UIManager.Instance.UpdateDefenseText();
    }
    public void LevelUpStats()
    {
        playerStats.AddTempFlat(StatType.MaxHealth, playerStats.HealthPerLevel);
        playerStats.AddTempFlat(StatType.Damage,    playerStats.DamagePerLevel);
        playerStats.AddTempFlat(StatType.Defense,   playerStats.DefensePerLevel);
        playerStats.AddTempFlat(StatType.Shield,    playerStats.ShieldPerLevel);

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
