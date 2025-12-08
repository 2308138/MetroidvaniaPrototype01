using UnityEngine;

public class BossAttackJumpHitbox : MonoBehaviour
{
    public BossState_AttackJump jumpState;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) jumpState.DealShockwaveDamage(collision);
    }
}