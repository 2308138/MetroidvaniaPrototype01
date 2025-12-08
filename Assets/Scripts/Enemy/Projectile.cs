using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 8F;
    public float damage = 1F;
    public float lifeTime = 3F;

    // --- RUNTIME VARIABLES --- //
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    public void Initialize(Vector2 direction) => moveDirection = direction.normalized;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void Start() => Destroy(gameObject, lifeTime);

    private void FixedUpdate() => rb.linearVelocity = moveDirection * speed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var target)) target.TakeDamage(damage, moveDirection);

        Destroy(gameObject);
    }
}