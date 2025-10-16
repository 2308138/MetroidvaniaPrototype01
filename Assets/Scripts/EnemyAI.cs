using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0F;

    private bool movingRight = true;
    private bool isChasing = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = 0F;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0F;
    public LayerMask wallLayer;

    [Header("Player Detection Settings")]
    public float detectionRange = 0F;
    public LayerMask playerLayer;

    private Rigidbody2D enemyRB;
    private Transform player;

    void Start()
    {
        enemyRB = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        if (player != null)
        {
            // --- DISTANCE CALCULATION ---
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // --- PLAYER DETECTION ---
            isChasing = distanceToPlayer <= detectionRange;

            // --- BEHAVIOR SELECTION ---
            if (isChasing)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }

            // --- FLIP SPRITE ---
            transform.localScale = new Vector3(movingRight ? 1 : -1, 1, 1);
        }
    }

    void Patrol()
    {
        // --- CHECK FOR GROUND AND WALLS ---
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        bool isWallAhead = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, wallCheckDistance, wallLayer);

        enemyRB.linearVelocity = new Vector2((movingRight ? 1 : -1) * moveSpeed, enemyRB.linearVelocity.y);

        // --- FLIP APPLICATION ---
        if (!isGroundAhead || isWallAhead)
        {
            Flip();
        }
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            // --- MOVE TOWARDS PLAYER ---
            float direction = player.position.x > transform.position.x ? 1 : -1;
            enemyRB.linearVelocity = new Vector2 (direction * moveSpeed * 1.2F, enemyRB.linearVelocity.y);
            movingRight = direction > 0;
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        if (wallCheck != null)
        {
            Vector3 direction = movingRight ? Vector3.right : Vector3.left;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + direction * wallCheckDistance);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}