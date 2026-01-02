using UnityEngine;

/// <summary>
/// Improved CharacterController-based player movement:
/// - Robust ground check (Physics.CheckSphere fallback to controller.isGrounded)
/// - Smooth crouch (height & center interpolation)
/// - Guards for missing references
/// - Keeps feet roughly in place while changing height
/// - Clear separation of look / movement / gravity
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraTransform; // assign main camera (child) in inspector
    [Tooltip("A small Transform placed roughly at the player's feet for ground checks. If null, a temporary point will be used.")]
    public Transform groundCheck;

    [Header("Movement Settings")]
    public float speed = 12f;
    public float gravity = -19.62f;
    public float jumpHeight = 2f;

    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 6f;
    [Tooltip("Controls how fast the CharacterController height interpolates")]
    public float heightChangeSpeed = 8f;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    [Range(-90f, 90f)]
    public float minLook = -90f;
    [Range(-90f, 90f)]
    public float maxLook = 90f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayers = ~0; // default: everything

    // internal state
    Vector3 velocity;
    float xRotation = 0f;
    bool isGrounded;
    float targetHeight;
    Transform runtimeGroundCheck;

    void Awake()
    {
        if (controller == null) controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // initialize height target from current controller height (or standingHeight)
        targetHeight = controller != null ? controller.height : standingHeight;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ensure controller center starts consistent with height
        if (controller != null)
            controller.center = new Vector3(0f, controller.height / 2f, 0f);

        // if groundCheck not assigned, create a runtime helper at feet
        if (groundCheck == null)
        {
            runtimeGroundCheck = new GameObject("GroundCheckRuntime").transform;
            runtimeGroundCheck.SetParent(transform);
            runtimeGroundCheck.localPosition = Vector3.zero;
            groundCheck = runtimeGroundCheck;
        }
    }

    void Update()
    {
        if (controller == null) return; // safety

        // --- GROUND CHECK (Physics.CheckSphere is usually more reliable) ---
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore)
                     || controller.isGrounded;

        if (isGrounded && velocity.y < 0f)
        {
            // small negative keeps the controller snug to slopes / ground
            velocity.y = -2f;
        }

        // --- LOOK / MOUSE ---
        HandleLook();

        // --- INPUT & MOVEMENT ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // determine current horizontal speed (crouch reduces speed)
        bool crouchRequested = Input.GetKey(KeyCode.LeftControl);
        float currentSpeed = crouchRequested ? crouchSpeed : speed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- JUMP ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- CROUCH (smooth interpolation of height & center) ---
        targetHeight = crouchRequested ? crouchHeight : standingHeight;
        if (!Mathf.Approximately(controller.height, targetHeight))
        {
            // compute previous height to adjust transform so feet stay roughly in place
            float previousHeight = controller.height;
            float newHeight = Mathf.Lerp(previousHeight, targetHeight, Time.deltaTime * heightChangeSpeed);
            controller.height = Mathf.Clamp(newHeight, 0.1f, standingHeight);

            // center should be half the height (works for upright capsule)
            controller.center = new Vector3(0f, controller.height / 2f, 0f);

            // Move the transform up/down by half the delta so the feet remain near the same world position.
            // This is conservative: transform adjustment is small because height is interpolated.
            float heightDelta = controller.height - previousHeight;
            transform.position += Vector3.up * (heightDelta * 0.5f);
        }

        // --- GRAVITY & FINAL MOVE ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        if (cameraTransform == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLook, maxLook);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void OnDrawGizmosSelected()
    {
        // show ground check sphere in scene view when object selected
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void OnDestroy()
    {
        if (runtimeGroundCheck != null)
        {
            Destroy(runtimeGroundCheck.gameObject);
        }
    }
}