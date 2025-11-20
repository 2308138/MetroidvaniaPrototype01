using System.Collections;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Damageable : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 0F;

    private float currentHealth;

    [Header("Invincibility / Stun Settings")]
    public float hitStunDuration = 0F;
    public float invincibilityTimeAfterHit = 0F;

    private bool isStunned = false;
    private bool isInvincible = false;

    [Header("Knockback Settings")]
    public float knockbackBackwardsForce = 0F;
    public float knockbackUpwardsForce = 0F;
    public float knockbackRecoveryTime = 0F;

    [Header("Death Settings")]
    public GameObject deathEffectPrefab;
    public float destroyDelay = 0F;

    private bool isDead = false;

    [Header("Feedback Settings")]
    public HitFeedback hitFeedback;

    // --- RUNTIME VARIABLES --- //
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        if (hitFeedback == null) hitFeedback = GetComponent<HitFeedback>();
    }

    // --- INTERFACE IMPLEMENTATION --- //
    [System.Obsolete]
    public void TakeDamage(float amount, Vector2 hitDirection)
    {
        if (isDead) return;
        if (isInvincible) return;
        if (amount <= 0) return;

        // --- DAMAGE CALCULATION --- //
        currentHealth -= amount;

        // --- VISUAL & SOUND EFFECTS --- //
        if (hitFeedback != null)
        {
            hitFeedback.PlayHitEffect(transform.position);
            hitFeedback.PlayFlashPop();
        }

        // --- KNOCKBACK AND STUN APPLICATION --- //
        StartCoroutine(ApplyKnockbackAndStun(hitDirection));

        // --- INVINCIBILITY APPLICATION --- //
        if (invincibilityTimeAfterHit > 0F) StartCoroutine(ApplyInvincibility(invincibilityTimeAfterHit));

        // --- CHECK IF ENTITY IS DEAD --- //
        if (currentHealth <= 0F && !isDead)
        {
            isDead = true;
            HandleDeath();
        }
    }

    IEnumerator ApplyKnockbackAndStun(Vector2 hitDirection)
    {
        if (rb == null) yield break;

        isStunned = true;
        rb.linearVelocity = Vector2.zero;
        Vector2 knockDirection = (-hitDirection.normalized + Vector2.up * 0.15F).normalized;
        Vector2 knock = knockDirection * knockbackBackwardsForce;
        knock.y += knockbackUpwardsForce;
        rb.AddForce(knock, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackRecoveryTime);
        isStunned = false;
    }

    IEnumerator ApplyInvincibility(float t)
    {
        isInvincible = true;
        yield return new WaitForSeconds(t);
        isInvincible = false;
    }

    [System.Obsolete]
    private void HandleDeath()
    {
        if (deathEffectPrefab != null)
        {
            var fx = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            var fxSr = fx.GetComponent<SpriteRenderer>();
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (fxSr != null && sr != null)
            {
                fxSr.sortingLayerID = sr.sortingLayerID;
                fxSr.sortingOrder = sr.sortingOrder + 1;
            }
        }

        foreach (var col in GetComponentsInChildren<Collider2D>()) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        Destroy(gameObject, destroyDelay);
    }
}