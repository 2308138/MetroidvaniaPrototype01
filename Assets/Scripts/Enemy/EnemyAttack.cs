using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damage = 0F;
    public float knockbackForce = 0F;
    public float attackRange = 0F;
    public float attackCooldown = 0F;

    private float nextAttackTime;

    [Header("Layer Settings")]
    public LayerMask playerLayer;

    // --- RUNTIME VARIABLES --- //
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        // --- START COOLDOWN --- //
        nextAttackTime = Time.deltaTime;

        // --- PERFORM ATTACK LOGIC --- //
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange && nextAttackTime <= 0F)
        {
            DoAttack();
            nextAttackTime = attackCooldown;
        }
    }

    private void DoAttack()
    {
        // --- PLAYER DETECTION CALCULATION --- //
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
        foreach (var hit in hits)
        {
            // --- SKIP IF COLLIDE WITH SELF HITBOX --- //
            if (hit.gameObject == gameObject) continue;

            // --- DAMAGE APPLICATION & HIT DIRECTION CALCULATION --- //
            IDamageable d = hit.GetComponent<IDamageable>();
            if (d != null)
            {
                Vector2 hitDir = (hit.transform.position - transform.position).normalized;
                d.TakeDamage(damage, hitDir);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}