using UnityEngine;
using UnityEngine.UI;

public class UI_ParryIndicator : MonoBehaviour
{
    [Header("UI Hook")]
    public Image glowImage;
    public float glowDuration = 0.2F;

    // --- RUNTIME VARIABLES --- //
    private float timer = 0F;

    public void TriggerParryWindow() => timer = glowDuration;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            float t = timer / glowDuration;
            glowImage.color = new Color(1F, 1F, 0F, t);
        }
        else glowImage.color = new Color(1F, 1F, 0F, 0F);
    }
}