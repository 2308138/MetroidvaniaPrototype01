using TMPro;
using UnityEngine;

public class UI_BossPhaseIndicator : MonoBehaviour
{
    [Header("UI Hook")]
    public TextMeshProUGUI phaseText;

    [Header("UI Settings")]
    public float fadeTime = 1F;

    // --- RUNTIME VARIABLES --- //
    private float timer = 0F;

    public void UpdatePhase(int phase)
    {
        phaseText.text = "BOSS PHASE: " + phase;
        timer = fadeTime;
        phaseText.color = new Color(1F, 0.2F, 0.2F, 1F);
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            float a = timer / fadeTime;
            phaseText.color = new Color(1F, 0.2F, 0.2F, a);
        }
    }
}