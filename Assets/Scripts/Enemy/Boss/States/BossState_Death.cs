using UnityEngine;

public class BossState_Death : BossState
{
    [Header("Death Settings")]
    public GameObject deathEffectPrefab;
    public float destroyDelay = 1.2F;

    // --- RUNTIME VARIABLES --- //
    private bool hasDied = false;

    public override void EnterState()
    {
        if (hasDied) return;

        hasDied = true;
        
        // --- SPAWN EFFECTS --- //
        if (deathEffectPrefab != null) GameObject.Instantiate(deathEffectPrefab, boss.transform.position, Quaternion.identity);

        // --- DISABLE COLLIDERS --- //
        foreach (var col in boss.GetComponents<Collider2D>()) col.enabled = false;

        // --- DISABLE OTHER SCRIPTS --- //
        foreach (var mb in boss.GetComponents<MonoBehaviour>()) if (mb != boss) mb.enabled = false;

        // --- DISABLE MOVEMENT --- //
        Rigidbody2D rb = boss.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector2.zero;
        }

        GameObject.Destroy(boss.gameObject, destroyDelay);
        Debug.Log("Boss has died!");

        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreen");
    }

    public override void UpdateState() { }

    public override void ExitState() { }
}