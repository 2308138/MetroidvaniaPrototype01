using System.Security.Cryptography;
using UnityEngine;

public class OLD_EnemyAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 0F;
    public float knockbackForce = 0F;
    public float attackRange = 0F;
    public float attackCooldown = 0F;

    private float nextAttackTime;

    [Header("Player Check")]
    public LayerMask playerLayer;

    private Transform player;
    private Rigidbody2D enemyRB;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyRB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        // --- DISTANCE CALCULATION ---
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // --- ATTACK LOGIC ---
        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void AttackPlayer()
    {
        // --- PLAYER DETECTION ---
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        
        foreach (Collider2D hit in hits)
        {
            // --- IGNORE SELF COLLIDER ---
            if (hit.gameObject == gameObject)
                continue;

            // --- DEBUG LOG ---
            Debug.Log($"{gameObject.name} attacked {hit.name}!");

            // --- DAMAGE APPLICATION
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector2 hitDirection = (hit.transform.position - transform.position).normalized;
                damageable.TakeDamage(damage, hitDirection);
            }

            // --- APPLY KNOCKBACK ---
            Rigidbody2D playerRB = hit.gameObject.GetComponent<Rigidbody2D>();
            if (playerRB != null)
            {
                Vector2 hitDirection = (playerRB.position - (Vector2)transform.position).normalized;
                Vector2 knockDirection = (hitDirection + Vector2.up * 0.2F).normalized;
                playerRB.linearVelocity = Vector2.zero;
                playerRB.AddForce(knockDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}