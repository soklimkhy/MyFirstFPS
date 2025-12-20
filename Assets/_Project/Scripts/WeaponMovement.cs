using UnityEngine;

public class WeaponMovement : MonoBehaviour
{
    [Header("Sway Settings")]
    public float smooth = 8f;
    public float swayMultiplier = 2f;

    [Header("Bob Settings")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;
    private float defaultY;
    private float timer = 0;

    void Start()
    {
        defaultY = transform.localPosition.y;
    }

    void Update()
    {
        // 1. WEAPON SWAY (Mouse follow)
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);
        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);

        // 2. WEAPON BOB (Movement bounce)
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            // Player is moving
            timer += Time.deltaTime * bobSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x,
                defaultY + Mathf.Sin(timer) * bobAmount, transform.localPosition.z);
        }
        else
        {
            // Idle
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x,
                Mathf.Lerp(transform.localPosition.y, defaultY, Time.deltaTime * bobSpeed), transform.localPosition.z);
        }
    }
}