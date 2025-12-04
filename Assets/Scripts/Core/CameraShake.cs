using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera Settings")]
    private Vector3 origin;
    private float duration = 0F;
    private float magnitude = 0F;

    private void Start() => origin = transform.localPosition;

    private void Update()
    {
        if (duration > 0F)
        {
            transform.localPosition = origin + (Vector3)Random.insideUnitCircle * magnitude;
            duration -= Time.unscaledDeltaTime;
        }

        else
        {
            transform.localPosition = origin;
            duration = 0F;
        }
    }

    public void Shake(float d, float m)
    {
        duration = d;
        magnitude = m;
    }
}