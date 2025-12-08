using UnityEngine;

public class BossState_AttackSlash : BossState
{
    [Header("Timing Settings")]
    public float windupTime = 0.4F;
    public float activeTime = 0.01F;
    public float recoveryTime = 0.5F;

    [Header("Attack Settigns")]
    public float slashDamage = 1F;
    public Collider2D slashHitbox;

    // --- RUNTIME VARIABLES --- //
    private float timer;
    private int phase = 0;

    public override void EnterState()
    {
        // --- HARD RESET --- //
        timer = windupTime;
        phase = 0;
        slashHitbox.enabled = false;
    }

    public override void UpdateState()
    {
        // --- TIMER DECREASE --- //
        timer -= Time.deltaTime;
        if (timer > 0) return;

        if (phase == 0)
        {
            phase = 1;
            slashHitbox.enabled = true;
            timer = activeTime;
            return;
        }

        if (phase == 1)
        {
            phase = 2;
            slashHitbox.enabled = false;
            timer = recoveryTime;
            return;
        }

        boss.SwitchState(boss.idleState);
    }

    public override void ExitState()
    {
        slashHitbox.enabled = false;
    }

    public void DealSlashDamage(Collider2D hit)
    {
        if (hit.TryGetComponent<IDamageable>(out var dmg))
        {
            Vector2 dir = (hit.transform.position - boss.transform.position).normalized;
            dmg.TakeDamage(slashDamage, dir);
        }
    }
}