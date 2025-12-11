using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed Settings")]
    public float moveSpeed = 8F;
    public float acceleration = 30F;
    public float airControl = 0.6F;

    [Header("Jump Settings")]
    public float jumpForce = 14F;
    public float variableJumpMultiplier = 0.5F;
    public float coyoteTime = 0.12F;
    public float jumpBufferTime = 0.12F;

    [Header("Check Settings")]
    public Transform groundCheck;
    public float groundRadius = 0.2F;
    public LayerMask groundLayer;

    public Transform wallCheck;
    public float wallDistance = 0.28F;
    public LayerMask wallLayer;

    [Header("Wall Movement Settings")]
    public float wallSlideSpeed = 1.6F;
    public Vector2 wallJumpForce = new Vector2(10F, 14F);
    public float wallJumpDuration = 0.18F;
    public float wallStickTime = 0.18F;

    // --- RUNTIME VARIABLES --- //
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float moveInput;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;
    private float lastGroundedTime;
    private float lastJumpPressedTime;
    private float wallStickTimer;

    public KnockbackReceiver knockbackReceiver;
    public Vector2 lastMoveDirection {  get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if (knockbackReceiver == null) knockbackReceiver = GetComponent<KnockbackReceiver>();
    }

    private void Update()
    {
        if (knockbackReceiver != null && knockbackReceiver.IsStunned()) return;

        // --- CHECK INPUT --- //
        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0) lastMoveDirection = new Vector2(moveInput, 0F);
        else lastMoveDirection = new Vector2(sr.flipX ? -1F : 1F, 0F);

        // --- FLIP CHECK --- //
        if (moveInput != 0F && sr != null) sr.flipX = moveInput < 0F;

        // --- TIMER CALCULATION --- //
        if (Input.GetButtonDown("Jump")) lastJumpPressedTime = jumpBufferTime;
        lastGroundedTime -= Time.deltaTime;
        lastJumpPressedTime -= Time.deltaTime;

        // --- CHECK GROUNDED --- //
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
            if (isGrounded) lastGroundedTime = coyoteTime;
        }

        // --- CHECK WALL --- //
        float facing = moveInput != 0 ? Mathf.Sign(moveInput) : (sr != null && sr.flipX ? -1F : 1F);
        Vector2 rayOrigin;

        if (wallCheck != null)
        {
            Vector2 local = wallCheck.localPosition;
            Vector2 mirroredLocal = new Vector3(local.x * facing, local.y);
            rayOrigin = (Vector2)transform.position + mirroredLocal;
        }
        else rayOrigin = transform.position;

        Vector2 rayDir = Vector2.right * facing;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, wallDistance, wallLayer);
        isTouchingWall = hit.collider != null;

        Debug.DrawRay(rayOrigin, rayDir * wallDistance, isTouchingWall ? Color.red : Color.green);

        // --- WALL STICK TIMER --- //
        if (isTouchingWall && !isGrounded && moveInput == Mathf.Sign(facing)) wallStickTimer = wallStickTime;
        else wallStickTimer = Mathf.Max(0F, wallStickTimer - Time.deltaTime);

        // --- JUMP TIMER CALCULATION --- //
        if (lastJumpPressedTime > 0F && lastGroundedTime > 0F)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            lastJumpPressedTime = 0F;
            lastGroundedTime = 0F;
        }

        // --- VARIABLE JUMP HEIGHT --- //
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0F) rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * variableJumpMultiplier);

        // --- CHECK IF WALL SLIDING --- //
        isWallSliding = isTouchingWall && !isGrounded && rb.linearVelocity.y < 0F;

        // --- WALL JUMP CALCULATION --- //
        if (Input.GetButtonDown("Jump") && (isWallSliding || wallStickTimer > 0F))
        {
            isWallJumping = true;
            Invoke(nameof(StopWallJump), wallJumpDuration);

            float wallDir = facing;
            Vector2 force = new Vector2(-wallDir * wallJumpForce.x, wallJumpForce.y);
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);

            if (sr != null) sr.flipX = wallDir > 0F;
        }
    }

    private void FixedUpdate()
    {
        if (knockbackReceiver != null && knockbackReceiver.IsStunned()) return;
        if (isWallJumping) return;

        // --- SPEED CALCULATION --- //
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = isGrounded ? acceleration : (acceleration * airControl);
        if (wallStickTimer > 0F && !isGrounded) accelRate *= 0.5F;
        float movement = speedDiff * accelRate;
        rb.AddForce(Vector2.right * movement);

        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed) rb.linearVelocity = new Vector2(Mathf.Sign(rb.linearVelocity.x) * moveSpeed, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0F) rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (1F - 1F) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0F && !Input.GetButton("Jump")) rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (variableJumpMultiplier - 1F) * Time.fixedDeltaTime;

        if (isWallSliding) rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
    }

    private void StopWallJump() => isWallJumping = false;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.cyan;
            float facing = (sr != null && sr.flipX) ? -1f : 1f;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * facing * wallDistance);
        }
    }
}