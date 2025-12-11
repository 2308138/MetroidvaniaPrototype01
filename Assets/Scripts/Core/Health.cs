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

    [Header("Worldspace Health Bar Hook")]
    public GameObject worldBarPrefab;

    // --- UI EVENTS --- //
    public event Action<float, float> onHealthChanged;
    public event Action<float> onDamaged;
    public event Action onDeath;

    // --- RUNTIME VARIABLES --- //
    private bool isDead = false;
    private UI_PlayerHealthBar playerUI;
    private UI_EnemyWorldspaceHealthBar enemyUI;
    private UI_Manager ui;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (hitResponder == null) hitResponder = GetComponent<HitResponder>();
        if (knockbackReceiver == null) knockbackReceiver = GetComponent<KnockbackReceiver>();
        if (CompareTag("Player")) playerUI = FindObjectOfType<UI_PlayerHealthBar>();
        if (!CompareTag("Player"))
        {
            Transform parent = GameObject.Find("WorldspaceUI").transform;
            GameObject barObj = Instantiate(worldBarPrefab, transform.position, Quaternion.identity, parent);
            enemyUI = barObj.GetComponentInChildren<UI_EnemyWorldspaceHealthBar>(true);
            if (enemyUI == null) enemyUI = barObj.GetComponent<UI_EnemyWorldspaceHealthBar>();
            if (enemyUI != null) enemyUI.enemy = transform;
        }
    }

    public void TakeDamage(float amount, Vector2 hitDirection)
    {
        if (isDead) return;

        currentHealth -= amount;

        // --- UI EVENTS --- //
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        onDamaged?.Invoke(amount);

        // --- DMG NUMBERS LOCATION --- //
        if (ui != null && ui.worldspaceCanvas != null && ui.damageNumberPrefab != null)
        {
            Vector3 worldPos = transform.position + Vector3.up * 1.5F;
            GameObject num = Instantiate(ui.damageNumberPrefab, ui.worldspaceCanvas.transform);
            num.transform.position = worldPos;
            num.GetComponent<UI_DamageNumbers>()?.Show(amount);
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

        // --- UPDATE PLAYER UI --- //
        if (playerUI != null) playerUI.SetHP(currentHealth, maxHealth);

        // --- UPDATE ENEMY UI --- //
        if (enemyUI != null) enemyUI.SetHealth(currentHealth, maxHealth);

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