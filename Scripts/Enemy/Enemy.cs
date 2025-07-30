using UnityEngine;

public class Enemy : MonoBehaviour
{
    public BaseMovement movement;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement.UpdateMovement();
    }
}
