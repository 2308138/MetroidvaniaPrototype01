using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class HitResponder : MonoBehaviour
{
    [Header("FX Settings")]
    public GameObject hitEffectPrefab;

    [Header("Flash Settings")]
    public Color flashColor = Color.white;
    public Color hurtColor = new Color(0F, 0F, 0F);
    public float flashDuration = 0F;

    [Header("Pop Settings")]
    public float popScale = 0F;
    public float popDuration = 0F;

    [Header("Shake Settings")]
    public CameraShake cameraShake;
    public float shakeDuration = 0F;
    public float shakeMagnitude = 0F;

    // --- RUNTIME VARIABLES --- //
    private SpriteRenderer sr;
    private Color originalColor;
    private Vector3 originalScale;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) originalColor = sr.color;
        originalScale = transform.localScale;
        if (cameraShake == null) cameraShake = FindObjectOfType<CameraShake>();
    }

    public void OnHit(Vector2 worldPos)
    {
        if (hitEffectPrefab != null)
        {
            var fx = Instantiate(hitEffectPrefab, worldPos, Quaternion.identity);
            var fxSr = fx.GetComponent<SpriteRenderer>();
            if (fxSr != null && sr != null)
            {
                fxSr.sortingLayerID = sr.sortingLayerID;
                fxSr.sortingOrder = sr.sortingOrder + 1;
            }
        }

        if (cameraShake != null) cameraShake.Shake(shakeDuration, shakeMagnitude);
    }

    public void PlayFlashPop()
    {
        if (sr != null) StartCoroutine(FlashCoroutine());
        StartCoroutine(PopCoroutine());
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
        float half = Mathf.Max(0.001F, popDuration) / 2F;
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