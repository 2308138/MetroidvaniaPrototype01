using UnityEngine;

public class BossState_Stagger : BossState
{
    [Header("Stagger Settings")]
    public float staggerDuration = 2F;

    // --- RUNTIME VARIABLES --- //
    private float timer;

    public override void EnterState()
    {
        // --- HARD RESET --- //
        timer = staggerDuration;

        // --- DISABLE ATTACK HITBOXES (IF ANY) --- //
        foreach (var col in boss.GetComponentsInChildren<Collider2D>()) if (col.isTrigger) col.enabled = false;
    }

    public override void UpdateState()
    {
        // --- TIMER DECREASE --- //
        timer -= Time.deltaTime;

        if (timer <= 0) boss.SwitchState(boss.idleState);
    }

    public override void ExitState()
    {
        // --- RE-ENABLED HITBOXES BEFORE NEXT ATTACK --- //
        foreach (var col in boss.GetComponentsInChildren<Collider2D>()) if (col.isTrigger) col.enabled = true;
    }
}