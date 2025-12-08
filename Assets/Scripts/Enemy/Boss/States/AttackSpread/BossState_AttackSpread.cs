using Unity.VisualScripting;
using UnityEngine;

public class BossState_AttackSpread : BossState
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int projectileCount = 5;
    public float spreadAngle = 60F;

    [Header("Timing Settings")]
    public float windupTime = 0.4F;
    public float recoveryTime = 0.5F;

    // --- RUNTIME VARIABLES --- //
    private float timer;
    private bool fired = false;

    public override void EnterState()
    {
        // --- HARD RESET --- //
        timer = windupTime;
        fired = false;
    }

    public override void UpdateState()
    {
        // --- TIMER DECREASE --- //
        timer -= Time.deltaTime;

        if (!fired && timer <= 0)
        {
            fired = true;
            FireProjectiles();
            timer = recoveryTime;
        }

        if (fired && timer <= 0) boss.SwitchState(boss.idleState);
    }

    public override void ExitState() { }

    private void FireProjectiles()
    {
        // --- ANGLE CALCULATION --- //
        float startAngle = -spreadAngle * 0.5F;
        float step = spreadAngle / (projectileCount - 1);

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = startAngle + step * i;
            Vector2 dir = Quaternion.Euler(0F, 0F, angle) * Vector2.right;

            var projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            var proj = projObj.GetComponent<Projectile>();
            proj.Initialize(dir, boss.gameObject);
        }
    }
}