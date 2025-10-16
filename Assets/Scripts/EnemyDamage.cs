using System.Security.Cryptography;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 0F;
    public float knockbackForce = 0F;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // --- DEBUG LOG ---
            Debug.Log("Player is hit!");

            // --- APPLY KNOCKBACK ---
            Rigidbody2D playerRB = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRB != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRB.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}