using System.Collections;
using UnityEngine;

public class OLD_PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 0F;

    private float currentHealth;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount, Vector2 hitDirection)
    {
        // --- CHECK INVINCIBILITY ---
        OLD_PlayerHitReaction hitReaction = GetComponent<OLD_PlayerHitReaction>();
        if (hitReaction != null && hitReaction.IsInvincible())
            return;

        // --- DAMAGE CALCULATION ---
        currentHealth -= amount;

        // --- HIT REACTION ---
        if (hitReaction != null)
        {
            Vector2 knockbackDirection = -hitDirection.normalized;
            hitReaction.TakeHit(knockbackDirection * 5F);
        }

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
}