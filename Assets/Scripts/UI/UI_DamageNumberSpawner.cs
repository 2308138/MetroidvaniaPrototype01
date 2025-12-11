using UnityEngine;

public class UI_DamageNumberSpawner : MonoBehaviour
{
    [Header("UI Hook")]
    public static UI_DamageNumberSpawner i;
    public GameObject damageNumberPrefab;
    public Canvas canvas;

    private void Awake() => i = this;

    public void Spawn(float dmg, Vector3 worldPos)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        var obj = Instantiate(damageNumberPrefab, canvas.transform);
        obj.transform.position = screenPos;

        obj.GetComponent<UI_DamageNumbers>().Show(dmg);
    }
}