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
    }

    public override void UpdateState()
    {
        // --- TIMER DECREASE --- //
        timer -= Time.deltaTime;

        if (timer <= 0) boss.SwitchState(boss.idleState);
    }

    public override void ExitState() { }
}