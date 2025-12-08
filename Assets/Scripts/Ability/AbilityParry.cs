using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class AbilityParry : MonoBehaviour
{
    [Header("Ability Settings")]
    public string requiredAbility = "AbilityParry";
    public float tapWindow = 0.16F;
    public float holdThreshold = 0.18F;
    public float blockMaxDuration = 2F;
    public float cooldown = 0.3F;
    public float parryKnockback = 6F;
    public float parryStun = 0.2F;
    public bool IsParryActive { get; private set; }
    public bool IsBlockActive { get; private set; }

    // --- RUNTIME VARIABLES --- //
    private bool canParry = true;
    private PlayerAbilityManager abilities;
    private ParryHitbox parryHitbox;
    private Coroutine holdCheckCoroutine;
    private Coroutine blockCoroutine;

    private void Awake()
    {
        abilities = GetComponent<PlayerAbilityManager>();
        parryHitbox = GetComponentInChildren<ParryHitbox>(true);
        if (parryHitbox != null) parryHitbox.playerParry = this;

        IsParryActive = false;
        IsBlockActive = false;
    }

    private void Update()
    {
        if (!abilities.HasAbility(requiredAbility)) return;
        if (!canParry) return;

        // --- CHECK FOR INPUT --- //
        if (Input.GetKeyDown(KeyCode.K) || Input.GetMouseButtonDown(1))
        {
            if (holdCheckCoroutine != null) StopCoroutine(holdCheckCoroutine);
            holdCheckCoroutine = StartCoroutine(HoldCheck());
        }

        // --- CHECK IF RELEASED EARLY TO TRANSITION TO TAP OR BLOCK "MODE" --- //
        if (Input.GetKeyUp(KeyCode.K) || Input.GetMouseButtonUp(1))
        {
            if (IsBlockActive) StopBlock();
            else if (holdCheckCoroutine != null)
            {
                StopCoroutine(holdCheckCoroutine);
                TriggerParryTap();
            }
        }
    }

    IEnumerator HoldCheck()
    {
        // --- TIMER CALCULATION --- //
        float t = 0F;
        while (t < holdThreshold)
        {
            if (!(Input.GetKey(KeyCode.K)) || Input.GetMouseButton(1)) yield break;
            t += Time.deltaTime;
            yield return null;
        }

        // --- TRANSITION TO BLOCK "MODE" --- //
        StartBlock();
    }

    private void TriggerParryTap() => StartCoroutine(ParryWindow());

    IEnumerator ParryWindow()
    {
        IsParryActive = true;
        EnableParryHitbox(true);

        yield return new WaitForSeconds(tapWindow);

        EnableParryHitbox(false);
        IsParryActive = false;

        // --- COOLDOWN RESET --- //
        canParry = false;
        yield return new WaitForSeconds(cooldown);
        canParry = true;
    }

    private void StartBlock()
    {
        if (blockCoroutine != null) StopCoroutine(blockCoroutine);
        IsBlockActive = true;
        EnableParryHitbox(true);
        blockCoroutine = StartCoroutine(BlockRoutine());
    }

    IEnumerator BlockRoutine()
    {
        // --- TIMER RESET --- //
        float elapsed = 0F;

        // --- HOLD BLOCK TIMER --- //
        while ((Input.GetKey(KeyCode.K) || Input.GetMouseButton(1)) && elapsed < blockMaxDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        StopBlock();

        // --- COOLDOWN RESET --- //
        canParry = false;
        yield return new WaitForSeconds(cooldown);
        canParry = true;
    }

    private void StopBlock()
    {
        if (blockCoroutine != null) StopCoroutine(blockCoroutine);
        IsBlockActive = false;
        EnableParryHitbox(false);
    }

    private void EnableParryHitbox(bool enable)
    {
        if (parryHitbox != null) parryHitbox.SetActive(enable);
    }

    public void OnMeleeParried(GameObject attackerRoot, Vector2 hitPoint) // --- REFERENCED ONLINE --- //
    {
        if (attackerRoot == null) return;

        var kb = attackerRoot.GetComponent<KnockbackReceiver>();
        if (kb != null)
        {
            // --- DIRECTION CALCULATION --- //
            Vector2 dir = (attackerRoot.transform.position - transform.position).normalized;
            kb.ApplyKnockback(dir * parryKnockback);
        }

        // --- DISABLE MOVEMENT AS A "STUN" --- //
        var movement = attackerRoot.GetComponent<EnemyMovement>();
        if (movement != null) StartCoroutine(TemporaryDisableComponent(movement, parryStun));
    }

    IEnumerator TemporaryDisableComponent(MonoBehaviour comp, float duration)
    {
        if (comp == null) yield break;

        comp.enabled = false;
        yield return new WaitForSeconds(duration);
        comp.enabled = true;
    }
}