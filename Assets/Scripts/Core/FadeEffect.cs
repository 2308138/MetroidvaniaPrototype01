using System.Collections;
using UnityEngine;

public class FadeEffect : MonoBehaviour
{
    [Header("Fade Effect Settings")]
    public CanvasGroup fade;
    public float speed = 2F;

    public void FadeIn() => StartCoroutine(FadeRoutine(0));
    public void FadeOut() => StartCoroutine(FadeRoutine(1));

    IEnumerator FadeRoutine(float target)
    {
        while (Mathf.Abs(fade.alpha - target) > 0.01F)
        {
            fade.alpha = Mathf.MoveTowards(fade.alpha, target, Time.deltaTime * speed);
            yield return null;
        }
    }
}