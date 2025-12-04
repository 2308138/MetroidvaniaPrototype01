using System.Collections;
using UnityEngine;

public class KnockbackReceiver : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackForce = 6F;
    public float knockbackUpward = 1.2F;
    public float recoveryTime = 0.12F;

    [Header("Stun Settings")]
    public bool blockDuringStun = true;
    public float stunTime = 0.12F;

    // --- RUNTIME VARIABLES --- //
    private Rigidbody2D rb;
    private bool isRecovering = false;
    private bool isStunned = false;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    public void ApplyKnockback(Vector2 hitDirection)
    {
        if (isRecovering) return;
        StartCoroutine(KnockbackCoroutine(hitDirection));
    }

    IEnumerator KnockbackCoroutine(Vector2 hitDirection)
    {
        isRecovering = true;
        isStunned = true;

        rb.linearVelocity = Vector2.zero;
        Vector2 knockDir = (-hitDirection.normalized + Vector2.up * 0.15F).normalized;
        Vector2 force = knockDir * knockbackForce;
        force.y += knockbackUpward;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(recoveryTime);
        isRecovering = false;

        yield return new WaitForSeconds(stunTime);
        isStunned = false;
    }

    public bool IsStunned() => isStunned;
}