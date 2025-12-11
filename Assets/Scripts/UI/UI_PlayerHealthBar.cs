using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHealthBar : MonoBehaviour
{
    [Header("UI Hook")]
    public Image healthFill;

    [Header("UI Settings")]
    public float smoothSpeed = 8F;

    // --- RUNTIME VARIABLES --- //
    private float targetFill = 1F;

    public void SetHealth(float current, float max) => targetFill = current / max;

    private void Update() => healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
}