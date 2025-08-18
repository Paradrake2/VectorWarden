using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillCardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkillCard skillCard;
    public Button button;
    public Image cardImage;

    public TextMeshProUGUI statsText;
    public void UpdateUI()
    {
        if (skillCard != null)
        {
            cardImage.sprite = skillCard.cardPicture;
            LevelUp levelUpInstance = FindFirstObjectByType<LevelUp>();
            if (levelUpInstance != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => levelUpInstance.SelectSkillCard(skillCard));
            }
        }
        gameObject.GetComponent<Image>().enabled = true;
        gameObject.GetComponent<Button>().enabled = true;
        gameObject.GetComponent<SkillCardUI>().enabled = true;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        statsText.text = GetStatsDescription();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        statsText.text = string.Empty; // Clear stats text when pointer exits
    }
    string GetStatsDescription()
    {
        string desc = skillCard.skillName + "\n";
        if (skillCard.statModifiers != null)
        {
            foreach (var mod in skillCard.statModifiers) {
                desc += $"{mod.statType}: {mod.flatAmount} (Flat), {mod.percentAmount * 100}% (Percent)\n";
            }
        }
        if (skillCard.skillType == SkillType.Projectile || skillCard.skillType == SkillType.AutoAttack)
        {
            GameObject projectilePrefab = skillCard.projectilePrefab;
            ProjectileData data = projectilePrefab.GetComponent<PlayerProjectile>().projectileData;
            if (ProjectileLevelTracker.Instance.GetTier(data) is ProjectileUpgrade.Tier tier)
            {
                if (tier.damageMultiplier != 1f) desc += $"DMG x{tier.damageMultiplier} ";
                if (tier.speedMultiplier != 1f) desc += $"SPD x{tier.speedMultiplier} ";
                if (tier.sizeMult != 1f) desc += $"Size x{tier.sizeMult} ";
                if (tier.flatDamage != 0f) desc += $"Flat DMG +{tier.flatDamage} ";
                if (tier.flatSpeed != 0f) desc += $"Flat SPD +{tier.flatSpeed} ";
                if (tier.explosionRadius != 0f) desc += $"Explosion +{tier.explosionRadius} ";
                if (tier.knockbackAddition != 0f) desc += $"Knockback +{tier.knockbackAddition} ";
                if (tier.piercingAddition != 0) desc += $"Pierce +{tier.piercingAddition} ";
                if (tier.projectileAdd != 0) desc += $"Projectiles +{tier.projectileAdd} ";
                if (tier.homingRange != 0f) desc += $"Homing +{tier.homingRange} ";
                if (tier.attackSpeedModifier != 0f) desc += $"AtkSpeed +{tier.attackSpeedModifier} ";
                if (tier.unlockExplosive) desc += $"Explosive ";
                if (tier.unlockHoming) desc += $"Homing ";
                if (tier.shotgunStyle) desc += $"Shotgun ";
            }
            else
            {
                desc += "No upgrade data available.\n";
            }
            if (data.attackSpeedModifier != 0f) desc += $"Attack speed modifer: {data.attackSpeedModifier}\n";
            if (data.damageMultiplier != 1f) desc += $"Damage multiplier: {data.damageMultiplier + 1}\n";
            if (data.baseSpeed != 0f) desc += $"Projectile speed: {data.baseSpeed}\n";
            if (data.pierceAmount != 0) desc += $"Pierce amount: {data.pierceAmount}\n";
            if (data.explosionRadius != 0f) desc += $"Explosion radius: {data.explosionRadius}\n";
            if (data.homingRange != 0f) desc += $"Homing range: {data.homingRange}\n";
            if (data.knockbackForce != 0f) desc += $"Knockback force: {data.knockbackForce}\n";
            if (data.projectileSize != 1f) desc += $"Projectile size: {data.projectileSize}\n";
            if (data.lifetime != 0f) desc += $"Lifetime: {data.lifetime}\n";
        }
        if (skillCard.skillType == SkillType.Random)
        {
            desc += $"Random Value Range: {skillCard.minValue} - {skillCard.maxValue}\n";
        }
        return desc;
    }
    void Start()
    {
        statsText = GameObject.Find("StatsText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
