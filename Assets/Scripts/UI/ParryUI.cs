using UnityEngine;

public class ParryUI : MonoBehaviour
{
    [Header("Parry UI Settings")]
    public CanvasGroup indicator; // --- REFERENCED ONLINE --- //
    public float fadeSpeed = 8F;

    public void Show() => indicator.alpha = 1F;

    public void Hide() => indicator.alpha = 0F;

    private void Update()
    {
        indicator.alpha = Mathf.Lerp(indicator.alpha, 0F, Time.deltaTime * fadeSpeed);
    }
}