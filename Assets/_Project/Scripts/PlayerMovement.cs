using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement Settings")]
    public float speed = 12f;
    public float gravity = -20f; // Standard Earth-like gravity
    public float jumpHeight = 2f;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform fpsCam;
    private float xRotation = 0f;

    [Header("Weapon Bonus")]
    public WeaponSwitcher weaponSwitcher;
    public float swordSpeedMultiplier = 1.2f;

    [Header("Animation")]
    public Animator skinAnimator;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // CRITICAL SETTINGS for CharacterController
        if (controller != null)
        {
            controller.skinWidth = 0.08f; // Prevents jitter and falling
            controller.minMoveDistance = 0f;
        }
    }

    void Update()
    {
        // 1. Precise Ground Check
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            // Reset velocity but keep a tiny downward force to stay "glued"
            velocity.y = -2f;
        }

        // 2. Mouse Look
        HandleLook();

        // 3. Horizontal Movement Input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float moveSpeed = speed;
        if (weaponSwitcher != null && weaponSwitcher.selectedWeapon == 1)
        {
            moveSpeed *= swordSpeedMultiplier;
        }

        // Calculate horizontal direction
        Vector3 move = transform.right * x + transform.forward * z;

        // 4. Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Apply Gravity
        velocity.y += gravity * Time.deltaTime;

        // 6. THE FIX: Combine and Move ONCE
        // Horizontal (move * moveSpeed) + Vertical (velocity)
        Vector3 finalFrameMotion = (move * moveSpeed) + velocity;

        // Final move call
        controller.Move(finalFrameMotion * Time.deltaTime);

        // 7. Animation
        if (skinAnimator != null)
        {
            float horizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
            skinAnimator.SetFloat("Speed", horizontalSpeed);
        }
    }

    private void HandleLook()
    {
        if (fpsCam == null) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        fpsCam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}