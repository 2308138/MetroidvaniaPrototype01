using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0F;
    public float acceleration = 0F;
    public float airControl = 0F;

    private float moveInput;

    [Header("Jump Settings")]
    public float jumpForce = 0F;
    public float variableJumpMultiplier = 0F;
    public float coyoteTime = 0F;
    public float jumpBufferTime = 0F;

    private float lastGroundedTime;
    private float lastJumpPressedTime;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0F;
    public LayerMask groundLayer;

    private bool isGrounded;

    [Header("Gravity Settings")]
    public float fallMultiplier = 0F;
    public float lowJumpMultiplier = 0F;

    [Header("Wall Check")]
    public Transform wallCheck;
    public float wallCheckDistance = 0F;
    public LayerMask wallLayer;

    [Header("Wall Slide + Jump Settings")]
    public float wallSlideSpeed = 0F;
    public Vector2 wallJumpForce = new Vector2(0F, 0F);
    public float wallJumpDuration = 0F;

    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;

    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;

    void Start()
    {
        playerRB = GetComponentInChildren<Rigidbody2D>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // --- HORIZONTAL INPUT ---
        moveInput = Input.GetAxisRaw("Horizontal");

        // --- FLIP SPRITE ---
        if (moveInput != 0)
        {
            playerSprite.flipX = moveInput < 0;
        }

        // --- JUMP INPUT ---
        if (Input.GetButtonDown("Jump"))
        {
            lastJumpPressedTime = jumpBufferTime;
        }

        // --- REDUCE TIMERS ---
        lastGroundedTime -= Time.deltaTime;
        lastJumpPressedTime -= Time.deltaTime;

        // --- CHECK GROUND ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- CHECK WALL ---
        float facingDirection = playerSprite.flipX ? -1 : 1;
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, wallCheckDistance, wallLayer);

        if (isGrounded)
        {
            lastGroundedTime = coyoteTime;
        }

        // --- PERFORM JUMP ---
        if (lastJumpPressedTime > 0 && lastGroundedTime > 0)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
            lastGroundedTime = 0;
            lastJumpPressedTime = 0;
        }

        // --- VARIABLE JUMP HEIGHT ---
        if (Input.GetButtonUp("Jump") && playerRB.linearVelocity.y > 0)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.y * variableJumpMultiplier);
        }

        // --- WALL SLIDE DETECTION ---
        if (isTouchingWall && !isGrounded && playerRB.linearVelocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        // --- PERFORM WALL JUMP ---
        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            isWallJumping = true;
            Invoke(nameof(StopWallJump), wallJumpDuration);

            float wallDirection = playerSprite.flipX ? -1 : 1;
            Vector2 force = new Vector2(-wallDirection * wallJumpForce.x, wallJumpForce.y);
            playerRB.linearVelocity = Vector2.zero;
            playerRB.AddForce(force, ForceMode2D.Impulse);

            playerSprite.flipX = wallDirection > 0;
        }
    }

    void FixedUpdate()
    {
        // --- HORIZONTAL MOVEMENT ---
        float targetSpeed = moveInput * moveSpeed;
        float SpeedDifference = targetSpeed - playerRB.linearVelocity.x;
        float accelerationRate = isGrounded ? acceleration : acceleration * airControl;
        float movement = SpeedDifference * accelerationRate;

        playerRB.AddForce(Vector2.right * movement);

        // --- CLAMP VELOCITY ---
        if (Mathf.Abs(playerRB.linearVelocity.x) > moveSpeed)
        {
            playerRB.linearVelocity = new Vector2(Mathf.Sign(playerRB.linearVelocity.x) * moveSpeed, playerRB.linearVelocity.y);
        }

        // --- CUSTOM GRAVITY APPLICATION ---
        if (playerRB.linearVelocity.y < 0)
        {
            playerRB.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (playerRB.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            playerRB.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        // --- WALL SLIDE APPLICATION ---
        if (isWallSliding)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, Mathf.Clamp(playerRB.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
    }

    void StopWallJump()
    {
        isWallJumping = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * Mathf.Sign(transform.localScale.x) * wallCheckDistance);
        }
    }
}