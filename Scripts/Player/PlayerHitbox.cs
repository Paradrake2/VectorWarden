using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public Player player;
    public void HitByEnemy(float damage, string enemyName)
    {
        player.TakeDamage(damage, enemyName);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
