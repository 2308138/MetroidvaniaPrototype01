using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [Header("HitStop Settings")]
    public static HitStop i;

    private void Awake() => i = this;

    public void Freeze(float duration) => StartCoroutine(FreezeRoutine(duration));

    IEnumerator FreezeRoutine(float duration)
    {
        float original = Time.timeScale;
        Time.timeScale = 0F;
        yield return new WaitForSeconds(duration);
        Time.timeScale = original;
    }
}