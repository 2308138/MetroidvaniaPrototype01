using System.Security.Cryptography;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
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
                damageable.TakeDamage(damage);
            }

            // --- APPLY KNOCKBACK ---
            Rigidbody2D playerRB = hit.gameObject.GetComponent<Rigidbody2D>();
            if (playerRB != null)
            {
                Vector2 knockbackDirection = (hit.transform.position - transform.position).normalized;
                playerRB.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}