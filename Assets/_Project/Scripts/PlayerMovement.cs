using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement Settings")]
    public float speed = 12f;
    public float gravity = -30f; // Made gravity stronger for a "heavier" feel
    public float jumpHeight = 2.5f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    public float crouchSpeed = 6f;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform fpsCam; // Drag Main Camera here
    private float xRotation = 0f;

    [Header("Weapon Bonus (AK Online Style)")]
    public WeaponSwitcher weaponSwitcher;
    public float swordSpeedMultiplier = 1.2f;

    [Header("Animation & Skin")]
    public Animator skinAnimator;
    public GameObject thirdPersonCamera;
    private bool isThirdPerson = false;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (weaponSwitcher == null)
            weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();

        if (fpsCam == null && Camera.main != null)
            fpsCam = Camera.main.transform;

        // Start the player a bit above their current position to avoid getting stuck in the floor
        controller.enabled = false;
        transform.position += Vector3.up * 0.1f;
        controller.enabled = true;
    }

    void Update()
    {
        // 1. GROUND CHECK
        // We use a small downward force (-2f) to keep isGrounded true while walking
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. MOUSE LOOK
        HandleLook();

        // 3. MOVEMENT CALCULATION
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float moveSpeed = GetCurrentMoveSpeed();

        // Calculate direction
        Vector3 move = transform.right * x + transform.forward * z;

        // 4. JUMPING
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 5. APPLY GRAVITY
        velocity.y += gravity * Time.deltaTime;

        // 6. COMBINED MOVEMENT (The "Anti-Fall" Fix)
        // We multiply the WASD move by speed and deltaTime, 
        // then add the vertical velocity multiplied by deltaTime.
        Vector3 finalMovement = (move * moveSpeed) + velocity;
        controller.Move(finalMovement * Time.deltaTime);

        // 7. UTILITY & ANIMATION
        HandleCrouch();
        HandleViewToggle();
        UpdateSkinAnimations();
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

    private float GetCurrentMoveSpeed()
    {
        float currentBase = Input.GetKey(KeyCode.LeftControl) ? crouchSpeed : speed;

        if (weaponSwitcher != null && weaponSwitcher.selectedWeapon == 1)
        {
            return currentBase * swordSpeedMultiplier;
        }

        return currentBase;
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            controller.height = crouchHeight;

        if (Input.GetKeyUp(KeyCode.LeftControl))
            controller.height = standingHeight;
    }

    private void HandleViewToggle()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isThirdPerson = !isThirdPerson;
            if (thirdPersonCamera != null) thirdPersonCamera.SetActive(isThirdPerson);
        }
    }

    private void UpdateSkinAnimations()
    {
        if (skinAnimator != null)
        {
            // Get speed from the controller's actual velocity
            float horizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
            skinAnimator.SetFloat("Speed", horizontalSpeed);
            skinAnimator.SetBool("isGrounded", isGrounded);
        }
    }
}