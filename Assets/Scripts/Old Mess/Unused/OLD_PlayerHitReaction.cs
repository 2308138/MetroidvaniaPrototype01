using System.Collections;
using UnityEngine;

public class OLD_PlayerHitReaction : MonoBehaviour
{
    [Header("Hit Reaction Settings")]
    public float knockbackDuration = 0F;
    public float invincibilityTime = 0F;
    public float flashInterval = 0F;

    private bool isInvincible = false;
    private bool isKnockedBack = false;

    private Rigidbody2D playerRB;
    private SpriteRenderer playerSprite;

    void Awake()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeHit(Vector2 knockbackForce)
    {
        if (isInvincible) return;

        // --- KNOCKBACK APPLICATION ---
        if (!isKnockedBack)
        {
            StartCoroutine(ApplyKnockback(knockbackForce));
        }

        // --- iFRAMES APPLICATION ---
        StartCoroutine(ApplyInvincibility());
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }

    IEnumerator ApplyKnockback(Vector2 force)
    {
        isKnockedBack = true;
        playerRB.linearVelocity = Vector2.zero;
        playerRB.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
    }

    IEnumerator ApplyInvincibility()
    {
        isInvincible = true;
        float elapsed = 0F;
        while (elapsed < invincibilityTime)
        {
            if (playerSprite != null)
                playerSprite.enabled = !playerSprite.enabled;

            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        if (playerSprite != null)
            playerSprite.enabled = true;

        isInvincible = false;
    }
}