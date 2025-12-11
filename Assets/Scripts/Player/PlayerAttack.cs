using System.Security.Cryptography;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(0.9F, 0.6F);
    public float damage = 1F;
    public float cooldown = 0.28F;
    public LayerMask enemyLayer;
    public GameObject hitEffectPrefab;

    // --- RUNTIME VARIABLES --- //
    private float cooldownTimer;
    private SpriteRenderer sr;

    private void Awake() => sr = GetComponentInChildren<SpriteRenderer>();

    private void Update()
    {
        // --- TIMER DECREASE --- //
        cooldownTimer -= Time.deltaTime;

        // --- CHECK INPUT --- //
        if (cooldownTimer <= 0F && (Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0)))
        {
            Attack();
            cooldownTimer = cooldown;
        }

        // --- ATTACK POINT CALCULATION --- //
        if (attackPoint != null && sr != null)
        {
            Vector3 lp = attackPoint.localPosition;
            lp.x = Mathf.Abs(lp.x) * (sr.flipX ? -1F : 1F);
            attackPoint.localPosition = lp;
        }
    }

    private void Attack()
    {
        if (attackPoint == null) return;

        // --- CHECK HITBOX --- //
        var hits = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0F, enemyLayer);
        foreach (var h in hits)
        {
            if (h.gameObject == gameObject) continue;
            var d = h.GetComponent<IDamageable>();
            if (d != null)
            {
                Vector2 dir = (h.transform.position - transform.position).normalized;
                d.TakeDamage(damage, dir);
            }

            if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, h.transform.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}