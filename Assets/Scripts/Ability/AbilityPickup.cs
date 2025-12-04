using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public string abilityName = "";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerAbilityManager abilities = collision.GetComponent<PlayerAbilityManager>();

        if (abilities != null)
        {
            abilities.UnlockAbility(abilityName);
            Destroy(gameObject);
        }
    }
}