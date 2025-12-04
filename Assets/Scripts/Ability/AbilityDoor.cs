using UnityEngine;

public class AbilityDoor : MonoBehaviour
{
    public string requiredAbility = "DoorKey";
    public Animator animator;
    public bool isOpen = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen) return;

        PlayerAbilityManager abilities = collision.GetComponent<PlayerAbilityManager>();
        if (abilities == null) return;

        if (abilities.HasAbility(requiredAbility)) OpenDoor();
        else Debug.Log("Door locked. Requires: " + requiredAbility);
    }

    private void OpenDoor()
    {
        isOpen = true;
        if (animator != null) animator.SetTrigger("Open");
        GetComponent<Collider2D>().enabled = false;

        Debug.Log("Door is opened!");
    }
}