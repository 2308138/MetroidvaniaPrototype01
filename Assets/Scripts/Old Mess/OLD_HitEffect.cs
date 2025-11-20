using UnityEngine;

public class OLD_HitEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float lifetime = 0F;
    public float scale = 0F;

    private SpriteRenderer effectSprite;

    void Start()
    {
        effectSprite = GetComponent<SpriteRenderer>();
        effectSprite.color = Color.white;
        transform.localScale = Vector3.one * scale;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // --- FADE OUT ---
        if (effectSprite != null)
        {
            Color c = effectSprite.color;
            c.a -= Time.deltaTime * 8F;
            effectSprite.color = c;
        }
    }
}