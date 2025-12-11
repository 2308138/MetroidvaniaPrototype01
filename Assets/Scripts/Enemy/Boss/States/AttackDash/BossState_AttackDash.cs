using UnityEngine;

public class BossState_AttackDash : BossState
{
    [Header("Dash Settings")]
    public float dashSpeed = 24F;
    public float dashDuration = 0.35F;

    [Header("Finisher Slash Settings")]
    public Collider2D finisherHitbox;
    public float finisherDamage = 1F;
    public float finisherActiveTime = 0.01F;

    // --- RUNTIME VARIABLES --- //
    private float timer;
    private Vector2 dashDirection;
    private int phase = 0;

    public override void EnterState()
    {
        // --- DIRECTION CALCULATION --- //
        var player = boss.GetPlayer();
        dashDirection = (player.position - boss.transform.position).normalized;

        // --- HARD RESET --- //
        timer = dashDuration;
        phase = 0;
        finisherHitbox.enabled = false;
    }

    public override void UpdateState()
    {
        // --- TIMER DECREASE --- //
        timer -= Time.deltaTime;

        if (timer > 0)
        {
            // --- PERFORM DASH --- //
            boss.transform.position += (Vector3)(dashDirection * dashSpeed * Time.deltaTime);
            return;
        }

        if (phase == 0)
        {
            // --- PERFORM SLASH --- //
            phase = 1;
            finisherHitbox.enabled = true;
            timer = finisherActiveTime;
            return;
        }

        if (phase == 1) finisherHitbox.enabled = false;
            
        boss.SwitchState(boss.idleState);
    }

    public override void ExitState()
    {
        finisherHitbox.enabled = false;
    }

    public void DealFinisherDamage(Collider2D hit)
    {
        if (hit.TryGetComponent<IDamageable>(out var dmg))
        {
            Vector2 dir = (hit.transform.position - boss.transform.position).normalized;
            dmg.TakeDamage(finisherDamage, dir);
        }
    }

    public override void OnParried()
    {
        // --- DISABLE ATTACK --- //
        finisherHitbox.enabled = false;

        // --- BUILD UP STAGGER --- //
        boss.AddStagger(boss.staggerGain);

        // --- PARRY FLASH --- //
        boss.GetComponent<HitResponder>()?.PlayParryFlash();

        if (!(boss.currentState is BossState_Stagger)) boss.SwitchState(boss.idleState);
    }
}