using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 10F;
    [HideInInspector] public float currentHealth;

    [Header("Hit / Death Settings")]
    public HitResponder hitResponder;
    public KnockbackReceiver knockbackReceiver;
    public GameObject deathPrefab;
    public float destroyDelay = 0.4F;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (hitResponder == null) hitResponder = GetComponent<HitResponder>();
        if (knockbackReceiver == null) knockbackReceiver = GetComponent<KnockbackReceiver>();
    }

    public void TakeDamage(float amount, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= amount;

        hitResponder?.OnHit(transform.position);
        hitResponder?.PlayFlashPop();

        if (knockbackReceiver != null) knockbackReceiver.ApplyKnockback(hitDirection);

        if (currentHealth <= 0F && !isDead)
        {
            isDead = true;
            Die();
        }

        Debug.Log($"{gameObject.name} took {amount} damage (HP: {currentHealth}/{maxHealth})");
    }

    private void Die()
    {
        if (deathPrefab != null) Instantiate(deathPrefab, transform.position, Quaternion.identity);

        foreach (var col in GetComponents<Collider2D>()) col.enabled = false;
        foreach (var mb in GetComponents<MonoBehaviour>()) if (mb != this) mb.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;
        }

        Destroy(gameObject, destroyDelay);
    }
}