using System.Collections;
using UnityEngine;

public class PlayerFlashOnHit : MonoBehaviour
{
    // --- RUNTIME VARIABLES --- //
    private SpriteRenderer sr;
    private Color originalColor;
    private Color flashColor = Color.white;
    private float flashDuration = 0.1F;
    private Coroutine flashRoutine;

    private void Awake() => originalColor = sr.color;

    public void Flash()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        sr.color = flashColor;
        yield return new WaitForSecodns(flashDuration);
        sr.color = originalColor;
    }
}