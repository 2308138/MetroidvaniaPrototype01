using UnityEngine;

public class BossState_AttackJump : BossState
{
    [Header("Jump Settings")]
    public float jumpApexHeight = 3F;
    public float jumpDuration = 1F;

    [Header("Shockwave Settings")]
    public Collider2D shockwaveHitbox;
    public float shockwaveActiveTime = 0.45F;
    public float shockwaveDamage = 1.2F;

    // --- RUNTIME VARIABLES --- //
    private float timer;
    private int phase = 0;
    private Vector3 startPos;

    public override void EnterState()
    {
        // --- HARD RESET --- //
        startPos = boss.transform.position;
        timer = jumpDuration;
        phase = 0;
        shockwaveHitbox.enabled = false;
    }

    public override void UpdateState()
    {
        // --- TIMER DECREASE --- //
        timer -= Time.deltaTime;

        if (phase == 0)
        {
            // --- IN AIR CALCULATION --- //
            float t = 1 - (timer / jumpDuration);
            float height = Mathf.Sin(t * Mathf.PI) * jumpApexHeight;

            boss.transform.position = startPos + new Vector3(0F, height, 0F);

            if (timer <= 0)
            {
                phase = 1;
                timer = shockwaveActiveTime;
                shockwaveHitbox.enabled = true;
            }

            return;
        }

        if (phase == 1)
        {
            if (timer <= 0)
            {
                shockwaveHitbox.enabled = false;
                boss.SwitchState(boss.idleState);
            }
        }
    }

    public override void ExitState()
    {
        shockwaveHitbox.enabled = false;
    }

    public void DealShockwaveDamage(Collider2D hit)
    {
        if (hit.TryGetComponent<IDamageable>(out var dmg))
        {
            Vector2 dir = (hit.transform.position - boss.transform.position).normalized;
            dmg.TakeDamage(shockwaveDamage, dir);
        }
    }
}