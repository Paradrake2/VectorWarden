using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    public Player player;
    public void HitByEnemy(float damage)
    {
        player.TakeDamage(damage);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
