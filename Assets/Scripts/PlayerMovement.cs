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
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}