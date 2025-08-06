using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    MaxHealth,
    Defense,
    Damage,
    DropRate,
    Mana,
    Knockback,
    GoldGain,
    Regeneration,
    XPGain,
    ManaRegeneration,
    DashDistance,
    DashNumber,
    PlayerMoveSpeed,
    AttackSpeed,
    AttackCooldown,
    Shield,
    // These need to be special effects
    ProjectileSpeed,
    PierceAmount,
    ExplosionRadius,
    HomingRange,
    ProjectileSize,
    
}
public class PlayerStats : MonoBehaviour
{
    public Dictionary<EquipmentSlot, Equipment> equippedItems = new();
    public List<Equipment> ownedGear = new List<Equipment>();
    public List<Equipment> existingGear = new List<Equipment>();
    public Equipment[] accessorySlots;
    public List<SkillCard> activeSkillCards = new List<SkillCard>();
    public List<SpecialEffectType> activeSpecialEffects = new List<SpecialEffectType>();
    public List<SkillCard> ownedSkillCards = new List<SkillCard>(); // Skill cards that have been acquired that the player can then take into battle

    public static PlayerStats Instance;
    public LevelUp levelUp;

    public int accessorySlotNum = 5;
    public int StartingCardCount = 1;
    public int Level = 1;
    public float XP = 0;
    public float BaseHealth = 100;
    public float BaseDefense = 0;
    public float BaseDamage = 1;
    public float PureDamage = 0;
    public float BaseMana = 100;
    public float BaseKnockback = 1;
    public float BaseAttackCooldown = 0.1f;
    public float BaseDropRate = 0.5f;
    public float BaseXPGain = 1f;
    public float BaseGoldGain = 1f;
    public float BaseRegeneration = 0f;
    public float BaseManaRegeneration = 1f;
    public float BaseProjectileSpeed = 10f;
    public float BaseShield = 0f;

    public float DashDistance = 2f;
    public int DashNumber = 2;
    public int BaseCardOptions = 3;
    public int BonusCardOptions = 0;
    public int BasePiercingAmount = 0;
    public float BaseExplosionRadius = 0;
    public float BaseHomingRange = 0;
    public float BaseProjectileSize = 1f;
    public float CurrentGold = 0f;
    public float BaseMoveSpeed = 5.0f;
    public float BaseAttackSpeed = 8f; // 8 attacks per second

    // Stats per level up
    public float HealthPerLevel = 10f;
    public float DamagePerLevel = 1f;
    public float DefensePerLevel = 0.5f;
    public int ShieldPerLevel = 5;


    public bool isDashing = false;

    public float CurrentDefense => CalculateStat(StatType.Defense);
    public float CurrentDamage => CalculateStat(StatType.Damage) + PureDamage;
    public float CurrentMaxHealth => CalculateStat(StatType.MaxHealth);
    public float CurrentMaxMana => BaseMana;
    public float CurrentMagicDamage => 0f;
    public float CurrentKnockback => CalculateStat(StatType.Knockback);
    public float CurrentAttackCooldown => 1f / CalculateStat(StatType.AttackSpeed);
    public float CurrentAttackSpeed => CalculateStat(StatType.AttackSpeed);
    public float CurrentRegeneration => BaseRegeneration;
    public float CurrentManaRegeneration => BaseManaRegeneration;
    public float CurrentProjectileSpeed => CalculateStat(StatType.ProjectileSpeed);
    public float CurrentDropRate => BaseDropRate;
    public float CurrentXPGain => CalculateStat(StatType.XPGain);
    public float CalculateDashDistance => CalculateStat(StatType.DashDistance);
    public float CalculateMaxShield => CalculateStat(StatType.Shield);
    public int CalculateDashNumber => DashNumber;
    public int CurrentPiercingAmount => CalculateStat(StatType.PierceAmount) > 0 ? (int)CalculateStat(StatType.PierceAmount) : 0;
    public float CurrentExplosionRadius => Mathf.Max(0f, CalculateStat(StatType.ExplosionRadius));
    public float CurrentHomingRange => Mathf.Max(0f, CalculateStat(StatType.HomingRange));
    public float CurrentProjectileSize => CalculateStat(StatType.ProjectileSize);
    public float CurrentPlayerMoveSpeed => CalculateStat(StatType.PlayerMoveSpeed);

    public float CurrentHealth;
    public int CurrentShield;
    public float CurrentMana;

    public float ShieldRegenCooldown = 5f;
    public float ShieldRegenRate = 5f; // how many seconds it takes to reach full shield
    private float lastShieldRegenTime = -99999f;
    private float shieldRegenBuffer = 0f;


    public float XpToNextLevel => Level * 2000;

    [Header("Current Stats (Read Only)")]
    [SerializeField] private float inspector_CurrentHealth;
    [SerializeField] private float inspector_CurrentMana;
    [SerializeField] private float inspector_CurrentDamage;
    [SerializeField] private float inspector_CurrentDefense;
    [SerializeField] private float inspector_CurrentProjectileSpeed;
    [SerializeField] private float inspector_CurrentPiercingAmount;
    [SerializeField] private float inspector_CurrentExplosionRadius;
    [SerializeField] private float inspector_CurrentHomingRange;
    [SerializeField] private float inspector_CurrentProjectileSize;
    [SerializeField] private float inspector_AttackCooldown = 0.5f; // Default attack cooldown
    [SerializeField] private float inspector_AttackSpeed = 8f; // Default attack speed
    [SerializeField] private float inspector_CurrentMaxShield;



    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        accessorySlots = new Equipment[accessorySlotNum];
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (levelUp == null)
        {
            levelUp = FindFirstObjectByType<LevelUp>();
            if (levelUp == null)
            {
                Debug.LogError("LevelUp component not found in the scene.");
            }
        }
        CurrentHealth = CurrentMaxHealth;
        CurrentMana = CurrentMaxMana;
        CurrentShield = (int)CalculateMaxShield;
        UIManager.Instance.UpdateShieldText();
    }

    void Update()
    {
        // Update inspector fields for debugging
        inspector_CurrentHealth = CurrentHealth;
        inspector_CurrentMana = CurrentMana;
        inspector_CurrentDamage = CurrentDamage;
        inspector_CurrentDefense = CurrentDefense;
        inspector_CurrentProjectileSpeed = CurrentProjectileSpeed;
        inspector_CurrentPiercingAmount = CurrentPiercingAmount;
        inspector_CurrentExplosionRadius = CurrentExplosionRadius;
        inspector_CurrentHomingRange = CurrentHomingRange;
        inspector_CurrentProjectileSize = CurrentProjectileSize;
        inspector_AttackCooldown = CurrentAttackCooldown;
        inspector_AttackSpeed = CurrentAttackSpeed;
        inspector_CurrentMaxShield = CalculateMaxShield;

        if (Time.time - lastShieldRegenTime > ShieldRegenCooldown && CurrentShield < CalculateMaxShield)
        {
            shieldRegenBuffer += GetShieldRegenRate() * Time.deltaTime;

            if (shieldRegenBuffer >= 1f)
            {
                int regenAmount = Mathf.FloorToInt(shieldRegenBuffer);
                CurrentShield = Mathf.Min(CurrentShield + regenAmount, (int)CalculateMaxShield);
                shieldRegenBuffer -= regenAmount;

                UIManager.Instance.UpdateShieldText();
                Debug.Log("Regen Shield: " + CurrentShield);
            }
        }

    }
    public void GainHealth(float amount)
    {
        CurrentHealth += amount;
        if (CurrentHealth > CurrentMaxHealth)
        {
            CurrentHealth = CurrentMaxHealth;
        }
        UIManager.Instance.UpdateHealthText();
    }

    public void GainXP(float amount)
    {
        XP += amount * CurrentXPGain;
        XPUIManager.Instance.UpdateXPText();
        XPUIManager.Instance.UpdateXPBarFill();
        while (XP >= XpToNextLevel)
        {
            XP -= XpToNextLevel;
            LevelUp();
        }
    }
    public List<GameObject> GetActiveProjectiles()
    {
        List<GameObject> projectiles = new List<GameObject>();
        foreach (var card in activeSkillCards)
        {
            if (card != null && card.projectilePrefab != null)
                projectiles.Add(card.projectilePrefab);
        }
        return projectiles;
    }
    public List<SpecialEffects> GetActiveSpecialEffects()
    {
        List<SpecialEffects> activeEffects = new List<SpecialEffects>();
        foreach (var card in activeSkillCards)
        {
            if (card != null)
            {
                foreach (var effect in card.specialEffects)
                {
                    activeEffects.Add(effect);
                }
            }
        }
        return activeEffects;
    }
    public void LevelUp()
    {
        Level++;
        levelUp.LevelUpPlayer(Level, GetNumCardOptions());

        CurrentShield = (int)CalculateMaxShield;
        UIManager.Instance.UpdateShieldText();
    }
    public float GetStat(StatType statType)
    {
        return CalculateStat(statType);
    }
    public int GetNumCardOptions()
    {
        return BaseCardOptions + BonusCardOptions;
    }
    float CalculateStat(StatType type)
    {
        float flat = 0f;
        float percent = 1f;

        foreach (var kvp in equippedItems)
        {
            var item = kvp.Value;
            if (item == null || item.modifiers == null) continue;
            foreach (var mod in item.modifiers)
            {
                if (mod.statType == type)
                {
                    flat += mod.flatAmount;
                    percent += mod.percentAmount;
                }
            }
        }
        foreach (var accessory in accessorySlots)
        {
            if (accessory == null || accessory.modifiers == null) continue;
            foreach (var mod in accessory.modifiers)
            {
                if (mod.statType == type)
                {
                    flat += mod.flatAmount;
                    percent += mod.percentAmount;
                }
            }
        }

        foreach (var card in activeSkillCards)
        {
            if (card == null) continue;
            foreach (var mod in card.statModifiers)
            {
                if (mod.statType == type)
                {
                    flat += mod.flatAmount;
                    percent += mod.percentAmount;
                }
            }
            // Debug.LogError("Checking card: " + card.skillName);
            var projData = card.projectileData;
            if (projData == null) continue;
            switch (type)
            {
                case StatType.AttackCooldown:
                    if (projData != null)
                    {
                        percent += projData.attackSpeedModifier;
                        //Debug.LogError("Attack Speed Modifier: " + projData.attackSpeedModifier);
                    }
                    break;
                case StatType.PierceAmount:
                    if (projData != null)
                    {
                        flat += projData.pierceAmount;
                    }
                    break;
                case StatType.ExplosionRadius:
                    if (projData != null)
                    {
                        flat += projData.explosionRadius;
                    }
                    break;
                case StatType.HomingRange:
                    if (projData != null)
                    {
                        flat += projData.homingRange;
                    }
                    break;
                case StatType.ProjectileSize:
                    if (projData != null)
                    {
                        flat += projData.projectileSize - 1f;
                    }
                    break;
                case StatType.Damage:
                    if (projData != null)
                    {
                        flat += projData.baseDamage;
                        percent += projData.damageMultiplier;
                    }
                    break;
            }
        }
        float baseValue = type switch
        {
            StatType.MaxHealth => BaseHealth,
            StatType.Defense => BaseDefense,
            StatType.Damage => BaseDamage,
            StatType.DropRate => BaseDropRate,
            StatType.Mana => BaseMana,
            StatType.Knockback => BaseKnockback,
            StatType.GoldGain => BaseGoldGain,
            StatType.Regeneration => BaseRegeneration,
            StatType.XPGain => BaseXPGain,
            StatType.ManaRegeneration => BaseManaRegeneration,
            StatType.DashDistance => DashDistance,
            StatType.DashNumber => DashNumber,
            StatType.ProjectileSpeed => BaseProjectileSpeed,
            StatType.AttackCooldown => BaseAttackCooldown,
            StatType.PierceAmount => BasePiercingAmount,
            StatType.ExplosionRadius => BaseExplosionRadius,
            StatType.HomingRange => BaseHomingRange,
            StatType.ProjectileSize => BaseProjectileSize,
            StatType.PlayerMoveSpeed => BaseMoveSpeed,
            StatType.AttackSpeed => BaseAttackSpeed,
            StatType.Shield => BaseShield,
            _ => 0
        };
        percent = Mathf.Max(0f, percent); // Ensure percent is not negative
        return (baseValue + flat) * percent;
    }

    public bool HasSpecialEffect(SpecialEffectType effectType)
    {
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == effectType)
            {
                return true;
            }
        }
        return activeSpecialEffects.Contains(effectType);
    }

    public int GetProjectileCount()
    {
        int projectileAmount = 1;
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.MultipleProjectiles)
            {
                projectileAmount += (int)effect.value;
            }
        }
        return projectileAmount;
    }

    public float GetLifestealAmount()
    {
        float lifestealAmount = 0f;
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.Lifesteal)
            {
                lifestealAmount += effect.value;
            }
        }
        return lifestealAmount;
    }
    public bool GetAutofireEnabled()
    {
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.AutoFire)
            {
                return true;
            }
        }
        return false;
    }
    public float GetCriticalChance()
    {
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.CriticalChance)
            {
                return effect.value;
            }
        }
        return 0f; // Default value if no effect is active
    }
    public float GetCriticalDamage()
    {
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.CriticalDamage)
            {
                return effect.value;
            }
        }
        return 0f; // Default value if no effect is active
    }
    public float GetExplosionRadius()
    {
        float val = 0f;
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.ExplosionRadius)
            {
                val += effect.value;
            }
        }
        return val; // Default value if no effect is active
    }

    public float GetHomingRange()
    {
        float val = 0f;
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.HomingRange)
            {
                val += effect.value;
            }
        }
        return val;
    }

    public int GetPiercingAmount()
    {
        int val = 0;
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.Piercing)
            {
                val += (int)effect.value;
            }
        }
        return val;
    }
    public float GetProjectileSize()
    {
        float val = 0f;
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.ProjectileSize)
            {
                val += effect.value;
            }
        }
        return val;
    }
    public float GetProjectileSpeed()
    {
        Debug.Log("Calculating Projectile Speed");
        float val = BaseProjectileSpeed;
        foreach (var effect in GetActiveSpecialEffects())
        {
            if (effect.effectType == SpecialEffectType.ProjectileSpeed)
            {
                val += effect.value;
            }
        }
        return val;
    }
    public int GetShieldRegenRate()
    {
        return Mathf.CeilToInt(CalculateMaxShield / ShieldRegenRate);
    }
    public void ResetShieldRegenCooldown()
    {
        lastShieldRegenTime = Time.time;
    }
}
