using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 0F;

    private float currentHealth;
    private bool isDead = false;

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
        isDead = true;
        // Player Death Logic Here
        Destroy(gameObject);
    }

    IEnumerator HitFlash()
    {
        SpriteRenderer playerSprite = GetComponentInChildren<SpriteRenderer>();
        if (playerSprite != null)
        {
            Color originalColor = playerSprite.color;
            playerSprite.color = Color.red;
            yield return new WaitForSeconds(0.1F);
            playerSprite.color = originalColor;
        }
    }
}