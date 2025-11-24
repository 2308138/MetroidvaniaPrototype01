using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0F;

    private bool movingRight = true;

    [Header("Environmental Check Settings")]
    public Transform groundCheck;
    public float groundCheckDistance = 0F;
    public LayerMask groundLayer;

    public Transform wallCheck;
    public float wallCheckDistance = 0F;
    public LayerMask wallLayer;

    [Header("Detection Settings")]
    public float detectionRange = 0F;
    public LayerMask playerLayer;

    // --- RUNTIME VARIABLES --- //
    private Rigidbody2D rb;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        // --- PLAYER DETECTION --- //
        float dist = Vector2.Distance(transform.position, player.position);
        bool isChasing = dist <= detectionRange;

        if (isChasing) Chase();
        else Patrol();

        // --- FLIP SPRITE --- //
        transform.localScale = new Vector3(movingRight ? 1F : -1F, 1F, 1F);
    }

    private void Patrol()
    {
        // --- LAYER CALCULATION --- //
        bool groundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool wallAhead = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);

        // --- PATROL MOVEMENT CALCULATION --- //
        rb.linearVelocity = new Vector2((movingRight ? 1F : -1F) * moveSpeed, rb.linearVelocity.y);

        // --- PATROL BOUNCE LOGIC --- //
        if (!groundAhead || wallAhead) Flip();
    }

    private void Chase()
    {
        // --- CHASE DIRECTION CALCULATION --- //
        float dir = player.position.x > transform.position.x ? 1F : -1F;

        // --- CHASE MOVEMENT CALCULATION --- //
        rb.linearVelocity = new Vector2(dir * moveSpeed * 1.2F, rb.linearVelocity.y);
        movingRight = dir > 0F;
    }

    private void Flip() => movingRight = !movingRight;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null) Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        if (wallCheck != null) Gizmos.DrawLine(wallCheck.position, wallCheck.position + (movingRight ? Vector3.right : Vector3.left) * wallCheckDistance);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}