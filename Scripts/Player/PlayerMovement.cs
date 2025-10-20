using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    public PlayerStats playerStats;
    private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 movement;
    public Vector2 CurrentVelocity { get; private set; }

    private Queue<Vector2> lastPositions = new Queue<Vector2>();
    private int maxPositions = 5; // Number of positions to keep track of

    public Vector2 SmoothedVelocity { get; private set; }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerStats = FindFirstObjectByType<PlayerStats>();
        moveSpeed = playerStats.CurrentPlayerMoveSpeed;
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - rb.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }
    void FixedUpdate()
    {
        moveSpeed = playerStats.CurrentPlayerMoveSpeed;
        Vector2 delta = movement.normalized * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        CurrentVelocity = delta / Time.fixedDeltaTime;

        lastPositions.Enqueue(CurrentVelocity);
        if (lastPositions.Count > maxPositions)
        {
            lastPositions.Dequeue();
        }
        Vector2 sum = Vector2.zero;
        foreach (var v in lastPositions) sum += v;
        SmoothedVelocity = sum/ lastPositions.Count;
    }
}
