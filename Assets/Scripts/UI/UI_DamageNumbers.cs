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

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        timer = lifeTime;
    }

    public void SetDamage(float dmg) => text.text = Mathf.RoundToInt(dmg).ToString();

    private void Update()
    {
        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0) Destroy(gameObject);
    }
}