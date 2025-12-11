using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Hook")]
    public Transform target;

    [Header("Follow Settings")]
    public float followSpeed = 6F;
    public Vector3 offset = new Vector3(0F, 0F, -10F);

    [Header("Look Ahead Settings")]
    public float lookAheadDistance = 2F;
    public float lookAheadSpeed = 4F;

    [Header("Vertical Deadzone Settings")]
    public float verticalDeadZone = 1.5F;

    // --- RUNTIME VARIABLES --- //
    private Rigidbody2D rb;
    private float currentLookAhead = 0F;

    private void Start()
    {
        if (target != null) rb = target.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + offset;

        // --- HORIZONTAL LOOK AHEAD --- //
        float dir = rb != null ? Mathf.Sign(rb.linearVelocity.x) : 0F;
        float speed = rb != null ? Mathf.Abs(rb.linearVelocity.x) : 0F;

        if (Mathf.Abs(speed) > 0.1) currentLookAhead = Mathf.Lerp(currentLookAhead, dir * lookAheadDistance, Time.deltaTime * lookAheadSpeed);
        else currentLookAhead = Mathf.Lerp(currentLookAhead, 0F, Time.deltaTime * lookAheadSpeed);

        targetPos.x += currentLookAhead;

        // --- VERTICAL DEADZONE LOGIC --- //
        float camY = transform.position.y;
        float targetY = target.position.y;

        if (Mathf.Abs(targetY - camY) > verticalDeadZone) targetPos.y = Mathf.Lerp(camY, targetY, Time.deltaTime * followSpeed);
        else targetPos.y = camY;

        // --- CAMERA SMOOTHING --- //
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetPos.x, targetPos.y, offset.z), Time.deltaTime * followSpeed);
    }
}