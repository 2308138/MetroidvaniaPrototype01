using UnityEngine;

public class BossAttackSlashHitbox : MonoBehaviour
{
    public BossState_AttackSlash slashState;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) slashState.DealSlashDamage(collision);
    }
}