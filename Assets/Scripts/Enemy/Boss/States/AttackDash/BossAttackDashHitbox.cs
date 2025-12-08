using UnityEngine;

public class BossAttackDashHitbox : MonoBehaviour
{
    public BossState_AttackDash dashState;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) dashState.DealFinisherDamage(collision);
    }
}