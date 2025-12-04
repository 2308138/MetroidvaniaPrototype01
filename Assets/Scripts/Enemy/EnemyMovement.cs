using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2F;

    [Header("Patrol Settings")]
    public Transform groundCheck;
    public Transform wallCheck;
    public float checkDistance = 0.2F;
    public LayerMask groundLayer;

    [Header("Chase Settings")]
    public float chaseRange = 6F;

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
        if (isChasing) Chase();
        else Patrol();

        // --- FLIP HANDLING --- //
        transform.localScale = new Vector3(movingRight ? 1F : -1F, 1F, 1F);
    }

    private void Patrol()
    {
        // --- CHECK FOR GROUND OR WALL --- //
        bool noGround = !Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
        bool wallAhead = Physics2D.Raycast(wallCheck.position, movingRight ? Vector2.right : Vector2.left, checkDistance, groundLayer);

        if (noGround || wallAhead) Flip();

        rb.linearVelocity = new Vector2((movingRight ? 1F : -1F) * moveSpeed, rb.linearVelocity.y);
    }

    private void Chase()
    {
        // --- DIRECTION CALCULATION --- //
        float dir = player.position.x > transform.position.x ? 1F : -1F;
        movingRight = dir > 0F;

        rb.linearVelocity = new Vector2(dir * moveSpeed * 1.3F, rb.linearVelocity.y);
    }

    private void Flip() => movingRight = !movingRight;
}