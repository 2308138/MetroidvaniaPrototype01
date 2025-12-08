using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class ParryHitbox : MonoBehaviour
{
    [Header("Parry Hook")]
    public AbilityParry playerParry;

    // --- RUNTIME VARIABLES --- //
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
        SetActive(false);
    }

    public void SetActive(bool on)
	{
        if (col != null) col.enabled = on;
	}

    private void OnTriggerEnter2D(Collider2D collision) // --- REFERENCED ONLINE --- //
    {
        if (playerParry == null) return;

        // --- PROJECTILE BEHAVIOR --- // (REFLECT)
        if (collision.TryGetComponent<Projectile>(out var proj))
        {
            // --- REFLECT WITH OWNER CHANGE --- //
            if (playerParry.IsParryActive || playerParry.IsBlockActive) proj.Reflect(gameObject.transform.root.gameObject);
            return;
        }

        // --- MELEE BEHAVIOR --- // (NEUTRALIZE)
        if (collision.TryGetComponent<Hitbox>(out var hitbox))
        {
            GameObject attackerRoot = collision.transform.root.gameObject;

            if (attackerRoot.CompareTag("Enemy"))
            {
                // --- DISABLE COLLIDER TO PREVENT DAMAGE --- //
                collision.enabled = false;

                // --- APPLY KNOCKBACK & STUN --- //
                playerParry.OnMeleeParried(attackerRoot, collision.ClosestPoint(transform.position));

                // --- FAIL-SAFE --- //
                StartCoroutine(ReenableColliderAfter(collision, 0.15F));
            }
            return;
        }
    }

    IEnumerator ReenableColliderAfter(Collider2D target, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (target != null) target.enabled = true;
    }
}