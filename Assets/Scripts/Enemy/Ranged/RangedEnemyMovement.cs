using UnityEngine;

public class RangedEnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2F;

    [Header("Patrol Settings")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float groundCheckDistance = 0.2F;
    public float wallCheckDistance = 0.28F;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    [Header("Chase Settings")]
    public float chaseRange = 8F;

    [Header("Distance Settings")]
    public float minDistance = 3F;
    public float maxDistance = 5F;

    // --- RUNTIME VARIABLES --- //
    private Rigidbody2D rb;
    private Transform player;
    private bool movingRight = true;
    private bool isChasing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void FixedUpdate()
    {
        if (!player) return;

        // --- DISTANCE CALCULATION --- //
        float dist = Vector2.Distance(transform.position, player.position);
        isChasing = dist <= chaseRange;

        // --- BEHAVIOR HANDLING --- //
        if (isChasing) Chase(dist);
        else Patrol();

        // --- FLIP HANDLING --- //
        transform.localScale = new Vector3(movingRight ? 1F : -1F, 1F, 1F);
    }

    private void Patrol()
    {
        // --- CHECK FOR GROUND OR WALL --- //
        bool noGround = !Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool wallAhead = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);

        if (noGround || wallAhead) Flip();

        rb.linearVelocity = new Vector2((movingRight ? 1F : -1F) * moveSpeed, rb.linearVelocity.y);
    }

    private void Chase(float distance)
    {
        // --- DIRECTION CALCULATION --- //
        float dir = player.position.x > transform.position.x ? 1F : -1F;
        movingRight = dir > 0F;

        // --- MOVEMENT LOGIC --- //
        if (distance < minDistance) rb.linearVelocity = new Vector2(-dir * moveSpeed, rb.linearVelocity.y); // --- BACK AWAY --- //
        else if (distance > maxDistance) rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y); // --- APPROACH --- //
        else rb.linearVelocity = new Vector2(0F, rb.linearVelocity.y); // --- STOP --- //
    }

    private void Flip() => movingRight = !movingRight;
}