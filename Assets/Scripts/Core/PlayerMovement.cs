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

    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpInputTime;

    [Header("Environmental Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0F;
    public LayerMask groundLayer;

    public Transform wallCheck;
    public float wallCheckDistance = 0F;
    public LayerMask wallLayer;

    [Header("Gravity Settings")]
    public float fallMultiplier = 0F;
    public float lowJumpMultiplier = 0F;

    [Header("Wall Slide + Jump Settings")]
    public float wallSlideSpeed = 0F;
    public Vector2 wallJumpForce = new Vector2(0F, 0F);
    public float wallJumpDuration = 0F;
    public float wallStickTime = 0F;

    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallStickTimer;

    // --- RUNTIME VARIABLES --- //
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // --- GET INPUT --- //
        moveInput = Input.GetAxisRaw("Horizontal");

        // --- SPRITE FLIP --- //
        if (moveInput != 0) sr.flipX = moveInput < 0;

        // --- JUMP BUFFER --- //
        if (Input.GetButtonDown("Jump")) lastJumpInputTime = jumpBufferTime;
        lastGroundedTime -= Time.deltaTime;
        lastJumpInputTime -= Time.deltaTime;

        // --- GROUND CHECK --- //
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded) lastGroundedTime = coyoteTime;

        // --- WALL CHECK --- //
        float facing = sr.flipX ? -1F : 1F;
        isTouchingWall = Physics2D.Raycast(wallCheck.position, Vector2.right * facing, wallCheckDistance, wallLayer);
        if (isTouchingWall && !isGrounded) wallStickTimer = wallStickTime;
        else wallStickTimer -= Time.deltaTime;

        // --- JUMP LOGIC --- //
        if (lastJumpInputTime > 0F && lastGroundedTime > 0F)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpInputTime = 0F;
            lastGroundedTime = 0F;
        }

        // --- VARIABLE JUMP HEIGHT --- //
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0F) rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * variableJumpMultiplier);

        // --- WALL SLIDE CHECK --- //
        isWallSliding = isTouchingWall && !isGrounded && rb.linearVelocity.y < 0F;

        // --- WALL JUMP LOGIC --- //
        if (Input.GetButtonDown("Jump") && (isWallSliding || wallStickTimer > 0F))
        {
            isWallJumping = true;
            Invoke(nameof(StopWallJump), wallJumpDuration);

            float wallDirection = sr.flipX ? -1F : 1F;
            Vector2 force = new Vector2(wallDirection * -wallJumpForce.x, wallJumpForce.y);
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);

            sr.flipX = wallDirection > 0F;
        }
    }

    private void FixedUpdate()
    {
        if (isWallJumping) return;

        // --- MOVEMENT MATH CALCULATION --- //
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = isGrounded ? acceleration : acceleration * airControl;
        if (wallStickTimer > 0F && isGrounded) accelRate *= 0.5F;
        float movement = speedDiff * accelRate;
        rb.AddForce(Vector2.right * movement);

        // --- CLAMP HORIZONTAL SPEED --- //
        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed) rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);

        // --- CUSTOM GRAVITY LOGIC --- //
        if (rb.linearVelocity.y < 0f) rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0f && !Input.GetButton("Jump")) rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;

        // --- CLAMP WALL SLIDE SPEED --- //
        if (isWallSliding) rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
    }

    private void StopWallJump() => isWallJumping = false;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            float facing = (sr != null && sr.flipX) ? -1F : 1F;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * facing * wallCheckDistance);
        }
    }
}