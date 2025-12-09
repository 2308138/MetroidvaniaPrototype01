using UnityEngine;
using UnityEngine.UI;

public class BossStaggerUI : MonoBehaviour
{
    [Header("Boss Stagger UI Settings")]
    public Slider staggerBar;
    public Image flashImage;

    public void SetStagger(float normalized)
    {
        staggerBar.value = normalized;

        if (normalized >= 1F) Flash();
    }

    private void Flash()
    {
        flashImage.color = new Color(1F, 1F, 1F, 1F);
    }

    private void Update()
    {
        flashImage.color = Color.Lerp(flashImage.color, new Color(1F, 1F, 1F, 0F), Time.deltaTime * 4F);
    }
}