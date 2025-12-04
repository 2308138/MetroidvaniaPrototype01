using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Hitbox attackHitbox;
    public float attackCooldown = 1F;
    public float attackRange = 1.5F;

    // --- RUNTIME VARIABLES --- //
    private float cooldownTimer = 0F;
    private Transform player;

    private void Awake() => player = GameObject.FindGameObjectWithTag("Player")?.transform;

    private void Update()
    {
        if (!player) return;

        // --- TIMER DECREASE --- //
        cooldownTimer -= Time.deltaTime;

        // --- DISTANCE CALCULATION --- //
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackRange && cooldownTimer <= 0F)
        {
            Attack();
            cooldownTimer = attackCooldown;
        }
    }

    private void Attack()
    {
        if (attackHitbox != null) attackHitbox.Activate();
    }
}