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
        if (skillCard.skillType == SkillType.Projectile)
        {
            GameObject projectilePrefab = skillCard.projectilePrefab;
            ProjectileData data = projectilePrefab.GetComponent<PlayerProjectile>().projectileData;

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
