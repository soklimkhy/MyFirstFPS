using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement Settings")]
    public float speed = 12f;
    public float gravity = -25f; // Stronger gravity for FPS feel
    public float jumpHeight = 2.5f;

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

        // REPO FIX: Ensure we start slightly above ground to prevent "Collider Snagging"
        if (controller != null)
        {
            controller.stepOffset = 0.3f;
            controller.skinWidth = 0.08f; // The "Magic" value to stop falling
        }
    }

    void Update()
    {
        // 1. Better Ground Check
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Clamp velocity when touching ground
        }

        // 2. Mouse Look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        fpsCam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 3. Horizontal Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float moveSpeed = (weaponSwitcher != null && weaponSwitcher.selectedWeapon == 1) ? speed * swordSpeedMultiplier : speed;

        Vector3 move = transform.right * x + transform.forward * z;

        // 4. Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. Gravity
        velocity.y += gravity * Time.deltaTime;

        // REPO FIX: Combine movement into ONE call to prevent physics tunneling
        Vector3 finalMovement = (move * moveSpeed) + velocity;
        controller.Move(finalMovement * Time.deltaTime);

        // 6. Animation
        if (skinAnimator != null)
        {
            float horizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
            skinAnimator.SetFloat("Speed", horizontalSpeed);
        }
    }
}