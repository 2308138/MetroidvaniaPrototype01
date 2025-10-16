using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Camera Shake Settings")]
    private Vector3 originalPos;
    private float shakeDuration = 0F;
    private float shakeMagnitude = 0.05F;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.unscaledDeltaTime;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }

    public void Shake(float duration = 0.01F, float magnitude = 0.05F)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}