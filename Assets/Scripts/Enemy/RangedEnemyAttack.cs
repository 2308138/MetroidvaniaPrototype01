using UnityEngine;

public class RangedEnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public float attackCooldown = 1.2F;
    public float attackRange = 6F;
    public Transform firePoint;

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
            Shoot();
            cooldownTimer = attackCooldown;
        }
    }

    private void Shoot()
    {
        if (!projectilePrefab || !firePoint) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // --- DIRECTION CALCULATION --- //
        Vector2 direction = (player.position - firePoint.position).normalized;
        Projectile projectile = proj.GetComponent<Projectile>();
        projectile?.Initialize(direction);
    }
}