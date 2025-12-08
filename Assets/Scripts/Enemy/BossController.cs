using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Settings")]
    public float maxHealth = 100F;
    public float phase2Threshold = 50F;
    public float phase3Threshold = 15F;
    [HideInInspector] public float currentHealth;
}