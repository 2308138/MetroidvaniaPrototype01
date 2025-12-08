using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "Boss/States/Idle")] // --- REFERENCED ONLINE --- //
public class BossState_Idle : BossState
{
    [Header("State Settings")]
    public float idleTimeMin = 0.6F;
    public float idleTimeMax = 1.3F;

    // --- RUNTIME VARIABLES --- //
    private float timer;

    public override void EnterState() => timer = Random.Range(idleTimeMin, idleTimeMax);

    public override void UpdateState()
    {
        // --- TIMER DECREASE --- //
        timer -= Time.deltaTime;

        if (timer > 0F) return;

        ChooseAttack();
    }

    private void ChooseAttack()
    {
        float r = Random.value;

        // --- PHASE 1 ATTACK BEHAVIOR --- //
        if (!boss.currentHealth <= boss.phase2Threshold)
        {
            if (r < 0.6F) boss.SwitchState(boss.slashAttack);
            else boss.SwitchState(boss.dashAttack);

            return;
        }

        // --- PHASE 2 & 3 ATTACK BEHAVIOR --- //
        if (r < 0.35F) boss.SwitchState(boss.slashAttack);
        if (r < 0.65F) boss.SwitchState(boss.dashAttack);
        if (r < 0.85F) boss.SwitchState(boss.jumpAttack);
        else boss.SwitchState(boss.spreadAttack);
    }

    public override void ExitState() { }
}