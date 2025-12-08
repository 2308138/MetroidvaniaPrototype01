using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Settings")]
    public float maxHealth = 100F;
    public float phase2Threshold = 50F;
    public float phase3Threshold = 15F;
    [HideInInspector] public float currentHealth;

    [Header("States")]
    public BossState currentState;
    public BossState_Idle idleState;
    public BossState_Stagger staggerState;
    public BossState_Death deathState;

    [Header("Phase 1 Attacks")]
    public BossState_AttackSlash slashAttack;
    public BossState_AttackDash dashAttack;

    [Header("Phase 2 Attacks")]
    public BossState_AttackJump jumpAttack;
    public BossState_AttackSpread spreadAttack;

    [Header("Stagger Settings")]
    public float staggerMeter = 0F;
    public float staggerMax = 10F;
    public float staggerGain = 1.5F;
    public float staggerStunDuration = 2F;

    // --- RUNTIME VARIABLES --- //
    private bool phase2Unlocked = false;
    private bool phase3Unlocked = false;
    private Transform player;

    private void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enabled = false;
    }

    public void ActivateBoss()
    {
        // --- START BOSS IN IDLE STATE --- //
        enabled = true;
        SwitchState(idleState);
    }

    public void SwitchState(BossState newState) // --- REFERENCED ONLINE --- //
    {
        if (currentState == newState) return;

        if (currentState != null) currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    private void Update()
    {
        if (currentState != null) currentState.UpdateState();
    }

    public void TakeDamage(float amount, Vector2 hitDir)
    {
        if (currentState == deathState) return;

        // --- DAMAGE CALCULATION --- //
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            SwitchState(deathState);
            return;
        }

        // --- PHASE UNLOCKS --- //
        if (!phase2Unlocked && currentHealth <= phase2Threshold)
        {
            phase2Unlocked = true;
            Debug.Log("Boss Phase 2.. START");
        }

        if (!phase3Unlocked && currentHealth <= phase3Threshold)
        {
            phase3Unlocked = true;
            Debug.Log("Boss Phase 3.. START");
        }
    }

    public void AddStagger(float amount)
    {
        staggerMeter += amount;

        if (staggerMeter >= staggerMax)
        {
            staggerMeter = 0F;
            TriggerStagger(staggerStunDuration);
        }
    }

    public void TriggerStagger(float duration)
    {
        staggerState.staggerDuration = duration;
        SwitchState(staggerState);
    }

    public Transform GetPlayer() => player;
}