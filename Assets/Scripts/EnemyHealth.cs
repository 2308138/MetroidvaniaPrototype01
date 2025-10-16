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
        SpriteRenderer enemySprite = GetComponentInChildren<SpriteRenderer>();
        if (enemySprite != null)
        {
            Color originalColor = enemySprite.color;
            enemySprite.color = Color.white;
            yield return new WaitForSeconds(0.1F);
            enemySprite.color = originalColor;
        }
    }
}