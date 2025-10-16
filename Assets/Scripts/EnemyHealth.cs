using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 0F;

    private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // --- DAMAGE CALCULATION ---
        currentHealth -= amount;

        // --- HIT FLASH APPLICATION ---
        StartCoroutine(HitFlash());

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
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1F);
            spriteRenderer.color = originalColor;
        }
    }
}