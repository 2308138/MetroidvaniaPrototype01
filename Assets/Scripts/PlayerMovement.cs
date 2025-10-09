using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 0F;
    public float jumpForce = 0F;

    private float moveInput;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0F;
    public LayerMask groundLayer;

    private bool isGrounded;

    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;

    void Start()
    {
        playerRB = GetComponentInChildren<Rigidbody2D>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
        }

        if (moveInput != 0)
        {
            playerSprite.flipX = moveInput < 0;
        }
    }

    void FixedUpdate()
    {
        playerRB.linearVelocity = new Vector2(moveInput * moveSpeed, playerRB.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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