using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 0F;

    private float currentHealth;

    [Header("Hit Feedback Settings")]
    public GameObject hitEffectPrefab;
    public float flashDuration = 0F;
    public Color flashColor = Color.white;
    public float hitStunDuration = 0F;

    private Color originalColor;
    private bool isStunned;

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
    }

    public void TakeDamage(float amount)
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

        // --- HIT FLASH APPLICATION ---
        StartCoroutine(HitFlash());

        // --- HIT STUN APPLICATION ---
        StartCoroutine(HitStun(hitStunDuration));

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
        if (enemySprite != null)
        {
            Color flash = flashColor;
            flash.a = originalColor.a;

            enemySprite.color = flash;
            yield return new WaitForSeconds(0.1F);
            enemySprite.color = originalColor;
        }
    }

    IEnumerator HitStun(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
}