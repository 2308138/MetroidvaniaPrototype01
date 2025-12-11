using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHealthBar : MonoBehaviour
{
    [Header("UI Hook")]
    public Slider hpSlider;
    public Slider chipSlider;

    [Header("UI Settings")]
    public float chipSpeed = 2F;

    public void SetHP(float current, float max) => hpSlider.value = current / max;

    private void Update()
    {
        if (chipSlider.value > hpSlider.value) chipSlider.value = Mathf.Lerp(chipSlider.value, hpSlider.value, Time.deltaTime * chipSpeed);
    }
}