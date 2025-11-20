using System.Collections;
using UnityEngine;

public class HitFeedback : MonoBehaviour
{
    [Header("Hit Effect")]
    public GameObject hitEffectPrefab;

    [Header("Flash Effect")]
    public Color flashColor = Color.white;
    public Color hurtColor = new Color(0F, 0F, 0F);
    public float flashDuration = 0F;

    private Color originalColor;

    [Header("Pop Effect")]
    public float popScale = 0F;
    public float popDuration = 0F;

    private Vector3 originalScale;

    // --- RUNTIME VARIABLES --- //
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
        originalScale = transform.localScale;
    }

    public void PlayHitEffect(Vector2 worldPosition)
    {
        if (hitEffectPrefab == null) return;
        var fx = Instantiate(hitEffectPrefab, worldPosition, Quaternion.identity);
        var fxSr = fx.GetComponent<SpriteRenderer>();
        if (fxSr != null && sr != null)
        {
            fxSr.sortingLayerID = sr.sortingLayerID;
            fxSr.sortingOrder = sr.sortingOrder + 1;
        }
    }

    public void PlayFlashPop()
    {
        if (sr != null)
        {
            StartCoroutine(FlashCoroutine());
            StartCoroutine(PopCoroutine());
        }
    }

    IEnumerator FlashCoroutine()
    {
        if (sr == null) yield break;
        sr.color = flashColor;
        yield return new WaitForSeconds(flashDuration * 0.3F);
        sr.color = hurtColor;
        yield return new WaitForSeconds(flashDuration * 0.7F);
        sr.color = originalColor;
    }

    IEnumerator PopCoroutine()
    {
        Vector3 target = originalScale * popScale;
        float half = popDuration / 2F;
        float t = 0F;
        while (t < half)
        {
            transform.localScale = Vector3.Lerp(originalScale, target, t / half);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0F;
        while (t < half)
        {
            transform.localScale = Vector3.Lerp(target, originalScale, t / half);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}