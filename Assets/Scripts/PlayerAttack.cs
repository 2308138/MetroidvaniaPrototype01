using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(0F, 0F);
    public float attackDamage = 0F;
    public float attackCooldown = 0F;
    public float knockbackForce = 0F;

    private float cooldownTimer;
    private bool isAttacking;

    [Header("Enemy Check")]
    public LayerMask enemyLayer;

    [Header("Visual Feedback")]
    public GameObject hitEffectPrefab;

    private SpriteRenderer playerSprite;

    void Start()
    {
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // --- REDUCE TIMERS ---
        cooldownTimer -= Time.deltaTime;

        // --- ATTACK INPUT ---
        if (cooldownTimer <= 0 && Input.GetKeyDown(KeyCode.J) || Input.GetButtonDown("Fire1"))
        {
            PerformAttack();
            cooldownTimer = attackCooldown;
        }

        // --- CHECK ATTACK POINT POSITION ---
        Vector3 localPos = attackPoint.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * (playerSprite.flipX ? -1 : 1);
        attackPoint.localPosition = localPos;
    }

    void PerformAttack()
    {
        // --- ENEMY DETECTION ---
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0F, enemyLayer);

        foreach (var hit in hits)
        {
            // --- IGNORE SELF COLLIDER ---
            if (hit.gameObject == gameObject)
                continue;

            // --- DAMAGE & KNOCKBACK APPLICATION ---
            var damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Vector2 hitDirection = (hit.transform.position - transform.position).normalized;
                damageable.TakeDamage(attackDamage, hitDirection);
            }

            // --- HIT FREEZE APPLICATION ---
            StartCoroutine(HitFreeze(0.05F));

            // --- PLAYER RECOIL APPLICATION ---
            StartCoroutine(PlayerRecoil(0.1F, 2F));

            // --- CAMERA SHAKE APPLICATION ---
            FindObjectOfType<CameraShake>()?.Shake(0.1F, 0.05F);

            // --- SPAWN HIT EFFECT ---
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.transform.position, Quaternion.identity);
            }
        }

        // --- ATTACK ANIMATION ---
        isAttacking = true;
        Invoke(nameof(ResetAttackState), 0.1F);
    }

    void ResetAttackState() => isAttacking = false;

    IEnumerator HitFreeze(float duration)
    {
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0F;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
    }

    IEnumerator PlayerRecoil(float duration, float strength)
    {
        Rigidbody2D playerRB = GetComponent<Rigidbody2D>();
        if (playerRB != null)
        {
            float direction = Mathf.Sign(transform.localScale.x) * -1F;
            Vector2 recoil = new Vector2(direction * strength, 0.5F);
            playerRB.AddForce(recoil, ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(duration);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(attackPoint.position, attackSize);
        }
    }
}