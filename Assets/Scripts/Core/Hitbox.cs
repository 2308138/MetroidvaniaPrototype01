using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("Hitbox Settings")]
    public float activeTime = 0.1F;
    public LayerMask hitLayer;
    public float damage = 1F;

    // --- RUNTIME VARIABLES --- //
    private Collider2D col;

    private void Awake() => col = GetComponent<Collider2D>();

    public void Activate()
    {
        StartCoroutine(DoActive());
    }

    IEnumerator DoActive()
    {
        col.enabled = true;
        yield return new WaitForSeconds(activeTime);
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.root == transform.root) return; // THIS IS REFERENCED ONLINE
        if (((1 << other.gameObject.layer) & hitLayer) == 0) return;

        var d = other.GetComponent<IDamageable>();
        if (d != null) d.TakeDamage(damage, (other.transform.position - transform.position).normalized);
    }
}