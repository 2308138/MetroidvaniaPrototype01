using UnityEngine;

public class AbilityDoor : MonoBehaviour
{
    [Header("Ability Settings")]
    public string requiredAbility = "AbilityDoor";
    public Animator animator;
    public bool isOpen = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (isOpen) return;

        PlayerAbilityManager abilities = collision.GetComponent<PlayerAbilityManager>();
        if (abilities != null && abilities.HasAbility(requiredAbility)) OpenDoor();
        else Debug.Log("Door locked. Requires: " + requiredAbility);
    }

    private void OpenDoor()
    {
        isOpen = true;
        if (animator != null) animator.SetTrigger("Open");
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;

        Debug.Log("Door is opened!");
    }
}