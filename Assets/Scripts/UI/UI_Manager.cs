using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [Header("Player UI Hook")]
    public UI_PlayerHealthBar playerHealthUI;
    public UI_ParryIndicator parryIndicator;

    [Header("Boss UI Hook")]
    public UI_BossHealthBar bossHealthUI;
    public UI_BossStaggerMeter bossStaggerUI;
    public UI_BossPhaseIndicator bossPhaseUI;

    [Header("DMG Numbers Hook")]
    public GameObject damageNumberPrefab;
    public Canvas worldspaceCanvas;

    // --- RUNTIME VARIABLES --- //
    private Health playerHealth;
    private BossController boss;

    private void Start()
    {
        // --- PLAYER UI --- //
        var playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Health>();
        if (playerHealth != null )
        {
            playerHealth.onHealthChanged += playerHealthUI.SetHP;
            playerHealth.onDamaged += SpawnDamageNumber;
        }

        // --- ENEMY UI --- //
        foreach (var h in FindObjectsOfType<Health>()) if (!h.CompareTag("Player")) h.onDamaged += SpawnDamageNumber;

        // --- BOSS UI --- //
        var boss = FindObjectOfType<BossController>();
        if (boss != null)
        {
            boss.onBossHealthChanged += bossHealthUI.SetHealth;
            boss.onStaggerChanged += bossStaggerUI.SetStagger;
            boss.onBossPhaseChanged += bossPhaseUI.UpdatePhase;
            boss.onBossDamaged += SpawnDamageNumber;
        }

        // --- PARRY UI --- //
        var parry = FindObjectOfType<AbilityParry>();
        if (parry != null) { }
    }

    private void SpawnDamageNumber(float dmg)
    {
        if (!damageNumberPrefab || !worldspaceCanvas) return;

        GameObject obj = Instantiate(damageNumberPrefab, worldspaceCanvas.transform);
        obj.GetComponent<UI_DamageNumbers>().Show(dmg);
    }
}