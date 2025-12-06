using System.Runtime.CompilerServices;
using UnityEngine;

public class AbilityDash : MonoBehaviour
{
    [Header("Ability Settings")]
    public string requiredAbility = "AbilityDash";
    public float dashForce = 12F;
    public float dashDuration = 0.12F;
    public float dashCooldown = 0.6F;

    // --- RUNTIME VARIABLES --- //
    private bool isDashing = false;
    private bool canDash = true;
    private Rigidbody2D rb;
    private PlayerMovement movement;
    private PlayerAbilityManager abilities;
    private Vector2 dashDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
        abilities = GetComponent<PlayerAbilityManager>();
    }

    private void Update()
    {
        if (isDashing) return;
        if (!canDash) return;
        if (!abilities.HasAbility(requiredAbility)) return;

        if (Input.GetKeyDown(KeyCode.LeftShift)) StartDash();
    }

    private void StartDash()
    {
        // --- DETERMINE DASH DIRECTION --- //
        dashDirection = movement.lastMoveDirection;

        // --- FAIL-SAFE DIRECTION --- //
        if (dashDirection == Vector2.zero)
        {
            float facing = movement.GetComponentInChildren<SpriteRenderer>().flipX ? -1F : 1F;
            dashDirection = new Vector2(facing, 0);
        }

        isDashing = true;
        canDash = false;

        // --- STOPS INPUTS WHEN DASHING --- //
        movement.enabled = false;

        // --- MOMENTUM CALCULATION --- //
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = dashDirection.normalized * dashForce;

        Invoke(nameof(EndDash), dashDuration);
    }

    private void EndDash()
    {
        isDashing = false;
        movement.enabled = true;

        // --- PRESERVE SOME MOMENTUM --- //
        rb.linearVelocity = new Vector2(0F, rb.linearVelocity.y);

        Invoke(nameof(ResetDash), dashCooldown);
    }

    private void ResetDash() => canDash = true;
}