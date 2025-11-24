using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackPoint;
    public Vector2 attackSize = new Vector2(0F, 0F);
    public float attackDamage = 0F;
    public float attackCooldown = 0F;

    private float cooldownTimer;

    [Header("Feedback Settings")]
    public GameObject hitEffectPrefab;

    [Header("Layer Settings")]
    public LayerMask enemyLayer;

    // --- RUNTIME VARIABLES --- //
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // --- START COOLDOWN --- //
        cooldownTimer -= Time.deltaTime;

        // --- ATTACK INPUT --- //
        if (cooldownTimer <= 0F && (Input.GetKeyDown(KeyCode.J)) || Input.GetButtonDown("Fire1"))
        {
            PerformAttack();
            cooldownTimer = attackCooldown;
        }

        // --- CALCULATE ATTACK POSITION --- //
        if (attackPoint != null && sr != null)
        {
            Vector3 localPos = attackPoint.localPosition;
            localPos.x = Mathf.Abs(localPos.x) * (sr.flipX ? -1F : 1F);
            attackPoint.localPosition = localPos;
        }
    }

    private void PerformAttack()
    {
        if (attackPoint == null) return;

        // --- ENEMY DETECTION CALCULATION --- //
        Collider2D[] hits = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0F, enemyLayer);
        foreach (var hit in hits)
        {
            // --- SKIP IF COLLIDE WITH SELF HITBOX --- //
            if (hit.gameObject == gameObject) continue;

            // --- DAMAGE APPLICATION & HIT DIRECTION CALCULATION --- //
            IDamageable d = hit.GetComponent<IDamageable>();
            if (d != null)
            {
                Vector2 hitDir = (hit.transform.position - transform.position).normalized;
                d.TakeDamage(attackDamage, hitDir);
            }

            // --- HIT EFFECT INSTANTIATION --- //
            if (hitEffectPrefab != null)
            {
                var fx = Instantiate(hitEffectPrefab, hit.transform.position, Quaternion.identity);
                var fxSr = fx.GetComponent<SpriteRenderer>();
                var targetSr = hit.GetComponentInChildren<SpriteRenderer>();
                if (fxSr != null && targetSr != null)
                {
                    fxSr.sortingLayerID = targetSr.sortingLayerID;
                    fxSr.sortingOrder = targetSr.sortingOrder + 1;
                }
            }
        }

        // --- ADDITIONAL MICRO FEEDBACK --- //
        StartCoroutine(HitFreeze(0.05F));
        FindObjectOfType<OLD_CameraShake>()?.Shake(0.08F, 0.04F);
        StartCoroutine(PlayerRecoil(0.06F, 1.6F));
    }

    IEnumerator HitFreeze(float duration)
    {
        float ori = Time.deltaTime;
        Time.timeScale = 0F;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = ori;
    }

    IEnumerator PlayerRecoil(float duration, float strength)
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float dir = Mathf.Sign(transform.localScale.x) * -1F;
            rb.AddForce(new Vector2(dir * strength, 0.5F), ForceMode2D.Impulse);
        }
        yield return new WaitForSeconds(duration);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(attackPoint.position, attackSize);
        }
    }
}