using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [Header("ScreenShake Settings")]
    public static ScreenShake i;

    // --- RUNTIME VARIABLES --- //
    private Vector3 originalPos;
    private float strength;

    private void Awake() => i = this;

    public void Shake(float amount, float duration)
    {
        strength = amount;
        CancelInvoke();
        Invoke("Stop", duration);
    }

    private void Stop() => strength = 0F;

    private void LateUpdate()
    {
        if (strength > 0) transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * strength;
        else transform.localPosition = originalPos;
    }
}