using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement Settings")]
    public float speed = 12f;
    public float gravity = -19.62f;
    public float jumpHeight = 2f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSpeed = 6f;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("Weapon Bonus (AK Online Style)")]
    public WeaponSwitcher weaponSwitcher; // Drag your WeaponHolder here in the Inspector
    public float swordSpeedMultiplier = 1.2f; // 20% boost

    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Automatically try to find the WeaponSwitcher if not assigned
        if (weaponSwitcher == null)
        {
            weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        }
    }

    void Update()
    {
        // 1. GROUND CHECK
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. MOUSE LOOK
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 3. SPEED CALCULATION (Including Sword Boost)
        float moveSpeed = GetCurrentMoveSpeed();

        // 4. MOVEMENT (WASD)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 5. JUMPING (Space)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 6. CROUCHING (Left Control)
        HandleCrouch();

        // 7. APPLY GRAVITY
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private float GetCurrentMoveSpeed()
    {
        // Determine base speed based on crouch
        float currentBase = Input.GetKey(KeyCode.LeftControl) ? crouchSpeed : speed;

        // Apply 20% boost if Sword is selected
        // Assuming selectedWeapon 0 = Gun, 1 = Sword (Knife)
        if (weaponSwitcher != null && weaponSwitcher.selectedWeapon == 1)
        {
            return currentBase * swordSpeedMultiplier;
        }

        return currentBase;
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            controller.height = crouchHeight;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            controller.height = standingHeight;
        }
    }
}