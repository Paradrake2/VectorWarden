using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private PlayerStats stats;
    public float damageCooldown = 0.1f;
    private float lastDamageTime = -999f;
    public bool isAttacking;
    public bool isMoving;
    void Start()
    {
        stats = PlayerStats.Instance;
    }
    public void TakeDamage(float damage, string enemyName)
    {
        if (Time.time - lastDamageTime < damageCooldown) return;
        if (stats.isDashing == true) return;
        lastDamageTime = Time.time;
        if (stats == null)
        {
            Debug.LogWarning("NO STATS");
        }
        stats.ResetShieldRegenCooldown();
        if (stats.CurrentShield > 0)
        {
            stats.CurrentShield -= Mathf.RoundToInt(damage);
            UIManager.Instance.UpdateShieldText();
            if (stats.CurrentShield < 0)
            {
                damage = -stats.CurrentShield; // Apply remaining damage after shield is depleted
                stats.CurrentShield = 0;
            }
            else
            {
                return; // No health lost, just shield
            }
        }
        float finalDamage = Mathf.Max(0, damage - stats.CurrentDefense);
        stats.CurrentHealth -= finalDamage;

        UIManager.Instance.UpdateHealthText();

        if (stats.CurrentHealth <= 0)
        {
            Die(enemyName);
        }
    }

    void Die(string name)
    {
        Debug.Log("Player has died");
        PlayerStats.Instance.ResetCards();
        Time.timeScale = 0;
        PostDeathUIScreen.Instance.ShowPostDeathScreen(name);
    }
    

}
