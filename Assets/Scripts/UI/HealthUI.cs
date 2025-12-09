using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Health UI Settings")]
    public Slider hpSlider;
    public Slider chipSlider;
    public float chipSpeed = 2F;

    public void SetHP(float normalized) => hpSlider.value = normalized;

    private void Update()
    {
        if (chipSlider.value > hpSlider.value) chipSlider.value = Mathf.Lerp(chipSlider.value, hpSlider.value, Time.deltaTime * chipSpeed);
    }
}