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
    public void TakeDamage(float damage)
    {
        Debug.Log("Player took damage: " + damage);
        if (Time.time - lastDamageTime < damageCooldown) return;
        if (stats.isDashing == true) return;
        lastDamageTime = Time.time;
        if (stats == null)
        {
            Debug.LogWarning("NO STATS");
        }
        float finalDamage = Mathf.Max(0, damage - stats.CurrentDefense);
        stats.CurrentHealth -= finalDamage;

        UIManager.Instance.UpdateHealthText();

        if (stats.CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player has died");
        SceneManager.LoadScene("Refine");
    }

}
