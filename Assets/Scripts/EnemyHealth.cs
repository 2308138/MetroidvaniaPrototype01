using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 0F;

    private float currentHealth;

    [Header("Hit Effect Settings")]
    public GameObject hitEffectPrefab;

    [Header("Hit Flash Settings")]
    public float flashDuration = 0F;
    public Color flashColor = Color.white;
    public Color hurtColor = new Color(0F, 0F, 0F);

    private Color originalColor;

    [Header("Hit Stun Settings")]
    public float hitStunDuration = 0F;

    private bool isStunned;

    [Header("Hit Pop Settings")]
    public float hitPopScale = 0F;
    public float popDuration = 0F;

    private Vector3 originalScale;

    [Header("Knockback Settings")]
    public float knockbackForce = 0F;
    public float knockbackUpwardForce = 0F;
    public float knockbackRecoveryTime = 0F;

    private bool isKnockedBack;

    private Rigidbody2D enemyRB;
    private SpriteRenderer enemySprite;

    void Start()
    {
        currentHealth = maxHealth;
        enemyRB = GetComponent<Rigidbody2D>();
        enemySprite = GetComponentInChildren<SpriteRenderer>();
        if (enemySprite != null)
        {
            originalColor = enemySprite.color;
        }
        originalScale = transform.localScale;
    }

    public void TakeDamage(float amount, Vector2 hitDirection)
    {
        // --- MULTI HIT PREVENTION ---
        if (isStunned) return;

        // --- DAMAGE CALCULATION ---
        currentHealth -= amount;

        // --- HIT EFFECT APPLICATION ---
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            var effectRenderer = effect.GetComponent<SpriteRenderer>();
            if (enemySprite != null && effectRenderer != null)
            {
                effectRenderer.sortingLayerID = enemySprite.sortingLayerID;
                effectRenderer.sortingOrder = enemySprite.sortingOrder + 1;
            }
        }

        // --- HIT FEEDBACK APPLICATION ---
        StartCoroutine(HitFlash());
        StartCoroutine(HitPop());
        StartCoroutine(HitStun(hitStunDuration));
        StartCoroutine(ApplyKnockback(hitDirection));


        // --- HANDLE DEATH  ---
        if (currentHealth <= 0)
        {
            Die();
        }

        // --- DEBUG LOG ---
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining HP: {currentHealth}");
    }

    void Die()
    {
        // --- DEATH SETTINGS ---
        Destroy(gameObject);
    }

    IEnumerator HitFlash()
    {
        if (enemySprite == null) yield break;

        enemySprite.color = flashColor;
        yield return new WaitForSeconds(flashDuration * 0.3F);

        enemySprite.color = hurtColor;
        yield return new WaitForSeconds(flashDuration * 0.7F);

        enemySprite.color = originalColor;
    }

    IEnumerator HitPop()
    {
        float elapsed = 0F;
        Vector3 targetScale = originalScale * hitPopScale;

        while (elapsed < popDuration / 2F)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / (popDuration / 2F));
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0F;
        while (elapsed < popDuration / 2F)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / (popDuration / 2F));
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }

    IEnumerator HitStun(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    IEnumerator ApplyKnockback(Vector2 hitDirection)
    {
        if (enemyRB == null) yield break;

        isKnockedBack = true;
        enemyRB.linearVelocity = Vector2.zero;

        Vector2 knockDirection = (-hitDirection.normalized + Vector2.up * 0.2F).normalized;
        Vector2 knockback = knockDirection * knockbackForce;
        knockback.y += knockbackUpwardForce;

        enemyRB.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackRecoveryTime);
        isKnockedBack = false;
    }
}