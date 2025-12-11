using UnityEngine;
using UnityEngine.UI;

public class UI_BossHealthBar : MonoBehaviour
{
    [Header("UI Hook")]
    public Image healthFill;
    public Image flashOverlay;

    [Header("UI Settings")]
    public float smoothSpeed = 6F;
    public float flashDuration = 0.25F;

    // --- RUNTIME VARIABLES --- //
    private float targetFill = 1F;
    private float flashTimer = 0F;

    public void SetHealth(float current, float max) => targetFill = current / max;

    public void Flash() => flashTimer = flashDuration;

    private void Update()
    {
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, Time.deltaTime * smoothSpeed);

        if (flashTimer > 0)
        {
            flashTimer -= Time.deltaTime;
            float t = flashTimer / flashDuration;
            flashOverlay.color = new Color(1F, 0F, 0F, t);
        }
        else flashOverlay.color = new Color(1F, 0F, 0F, 0F);
    }
}