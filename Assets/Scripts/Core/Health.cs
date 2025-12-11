using System;
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

    // --- UI EVENTS --- //
    public event Action<float, float> onHealthChanged;
    public event Action<float> onDamaged;
    public event Action onDeath;

    // --- RUNTIME VARIABLES --- //
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

        // --- FIND WORLDSPACE BAR --- //
        var ws = GetComponentInChildren<UI_EnemyWorldspaceHealthBar>();
        if (ws != null) ws.SetHealth(currentHealth, maxHealth);

        // --- UI EVENTS --- //
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onDamaged?.Invoke(amount);

        // --- DMG NUMBERS LOCATION --- //
        UI_Manager ui = FindObjectOfType<UI_Manager>();
        if (ui != null && ui.worldspaceCanvas != null && ui.damageNumberPrefab != null)
        {
            GameObject obj = Instantiate(ui.damageNumberPrefab, ui.worldspaceCanvas.transform);
            obj.GetComponent<UI_DamageNumbers>().Show(amount);
            Vector3 screen = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1F);
            obj.transform.position = screen;
        }

        // --- VFX + KNOCKBACK APPLICATION --- //
        hitResponder?.OnHit(transform.position);
        hitResponder?.PlayFlashPop();
        if (knockbackReceiver != null) knockbackReceiver.ApplyKnockback(hitDirection);

        if (currentHealth <= 0F && !isDead)
        {
            isDead = true;
            onDeath?.Invoke();
            Die();
        }

        // --- SPAWN DAMAGE NUMBERS --- //
        UI_DamageNumberSpawner.i?.Spawn(Mathf.RoundToInt(amount), transform.position);

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

        if (CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreen");
            return;
        }

        Destroy(gameObject, destroyDelay);
    }
}