using TMPro;
using UnityEngine;

public class UI_DamageNumbers : MonoBehaviour
{
    [Header("UI Hook")]
    public TMP_Text text;

    // --- RUNTIME VARIABLES --- //
    private float lifeTime = 0.6F;
    private float speed = 12F;
    private float fadeSpeed = 2F;

    private void Awake()
    {
        if (text == null) text = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (text == null) return;

        lifeTime -= Time.deltaTime;

        transform.Translate(Vector3.up * speed * Time.deltaTime);
        float alpha = Mathf.Clamp01(lifeTime * fadeSpeed);
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Show(float value) => text.text = value.ToString();
}