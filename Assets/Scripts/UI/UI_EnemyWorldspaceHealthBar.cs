using UnityEngine;
using UnityEngine.UI;

public class UI_EnemyWorldspaceHealthBar : MonoBehaviour
{
    [Header("UI Hook")]
    public Image healthFill;
    public Transform enemy;
    public Vector3 offset = new Vector3(0F, 1.5F, 0F);

    // --- RUNTIME VARIABLES --- //
    private Camera cam;
    private float targetFill = 1F;

    private void Start() => cam = Camera.main;

    public void SetHealth(float current, float max) => targetFill = current / max;

    private void LateUpdate()
    {
        transform.position = enemy.position + offset;
        transform.LookAt(transform.position + cam.transform.forward);
        
        healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, Time.deltaTime * 8F)
    }
}