using System.Collections;
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
    private GameObject owner;
    private Collider2D col;
    private SpriteRenderer sr;

    public void Initialize(Vector2  direction, GameObject ownerObject)
    {
        moveDirection = direction.normalized;
        owner = ownerObject;

        StartCoroutine(SetupIgnoreCollisions());

        // --- FLIP SPRITE --- //
        if (sr != null) sr.flipX = moveDirection.x < 0F;
    }

    IEnumerator SetupIgnoreCollisions() // --- REFERENCED ONLINE --- //
    {
        yield return null;

        if (owner == null || col == null) yield break;

        // --- IGNORE SELF COLLIDER --- //
        var ownerColliders = owner.GetComponents<Collider2D>();
        foreach (var oc in ownerColliders) Physics2D.IgnoreCollision(col, oc, true);

        // --- IGNORE ALL TAGGED COLLIDERS --- //
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var e in enemies)
        {
            if (e == owner) continue;

            var ecs = e.GetComponents<Collider2D>();
            foreach (var ec in ecs) Physics2D.IgnoreCollision(col, ec, true);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start() => Destroy(gameObject, lifeTime);

    private void FixedUpdate() => rb.linearVelocity = moveDirection * speed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner != null && collision.transform.root == owner.transform.root) return;
        if (collision.CompareTag("Enemy")) return;

        if (collision.TryGetComponent<IDamageable>(out var target)) target.TakeDamage(damage, moveDirection);

        Destroy(gameObject);
    }
}