using TMPro;
using UnityEngine;

public class UI_DamageNumbers : MonoBehaviour
{
    [Header("UI Settings")]
    public float riseSpeed = 1.2F;
    public float lifeTime = 0.8F;

    // --- RUNTIME VARIABLES --- //
    private TextMeshProUGUI text;
    private float timer;

    public void Show(float dmg)
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = Mathf.RoundToInt(dmg).ToString();
        timer = lifeTime;
    }

    private void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        timer -= Time.deltaTime;
        float alpha = timer / lifeTime;

        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

        if (timer <= 0) Destroy(gameObject);
    }
}