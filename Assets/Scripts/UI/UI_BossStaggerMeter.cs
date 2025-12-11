using UnityEngine;
using UnityEngine.UI;

public class UI_BossStaggerMeter : MonoBehaviour
{
    [Header("UI Hook")]
    public Image staggerFill;
    public RectTransform pulseObject;

    [Header("UI Settings")]
    public float pulseScale = 1.25F;
    public float pulseSpeed = 6F;

    // --- RUNTIME VARIABLES --- //
    private float targetFill = 0F;

    public void SetStagger(float current, float max) => targetFill = current / max;

    private void Update()
    {
        staggerFill.fillAmount = Mathf.Lerp(staggerFill.fillAmount, targetFill, Time.deltaTime * 10F);

        if (targetFill >= 1)
        {
            float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1); // --- REFERENCED ONLINE --- //
            pulseObject.localScale = new Vector3(scale, scale, 1F);
        }
        else pulseObject.localScale = Vector3.Lerp(pulseObject.localScale, Vector3.one, Time.deltaTime * 8F);
    }
}