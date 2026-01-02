using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Movement Settings")]
    public float speed = 12f;
    public float gravity = -20f;
    public float jumpHeight = 2.5f;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public Transform fpsCam;
    private float xRotation = 0f;

    [Header("Weapon Bonus")]
    public WeaponSwitcher weaponSwitcher;
    public float swordSpeedMultiplier = 1.2f;

    [Header("Animation & Skin")]
    public Animator skinAnimator;
    public GameObject thirdPersonCamera;
    private bool isThirdPerson = false;

    [Header("Safety Settings")]
    public float fallLimit = -20f; // If Y position is lower than this, teleport back
    public Vector3 spawnPoint = new Vector3(0, 5, 0);

    private Vector3 playerVelocity;
    private bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (controller == null) controller = GetComponent<CharacterController>();
        if (weaponSwitcher == null) weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        if (fpsCam == null && Camera.main != null) fpsCam = Camera.main.transform;

        // FIXED VALUES: These must be exactly this for imported maps
        controller.skinWidth = 0.08f;
        controller.minMoveDistance = 0f;
        controller.center = new Vector3(0, 1, 0); // Sets the capsule feet correctly
    }

    void Update()
    {
        // 1. SAFETY TELEPORT (If you fall through the map)
        if (transform.position.y < fallLimit)
        {
            TeleportToSafety();
            return;
        }

        // 2. GROUND CHECK
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // 3. MOUSE LOOK
        HandleLook();

        // 4. MOVEMENT INPUT
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float currentSpeed = (weaponSwitcher != null && weaponSwitcher.selectedWeapon == 1) ? speed * swordSpeedMultiplier : speed;

        // 5. JUMPING
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 6. APPLY GRAVITY
        playerVelocity.y += gravity * Time.deltaTime;

        // 7. COMBINED MOVEMENT
        Vector3 moveDirection = transform.right * x + transform.forward * z;
        Vector3 finalMove = (moveDirection * currentSpeed) + playerVelocity;

        controller.Move(finalMove * Time.deltaTime);

        // 8. UTILITY & ANIMATION
        HandleViewToggle();
        UpdateSkinAnimations();
    }

    private void TeleportToSafety()
    {
        controller.enabled = false; // Must disable controller to move transform manually
        transform.position = spawnPoint;
        playerVelocity = Vector3.zero;
        controller.enabled = true;
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
            float horizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
            skinAnimator.SetFloat("Speed", horizontalSpeed);
            skinAnimator.SetBool("isGrounded", isGrounded);
        }
    }
}