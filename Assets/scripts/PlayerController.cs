using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.8f;
    public float verticalSpeed = 5f;
    public bool flyMode = false;

    [Header("Jump")]
    public float jumpHeight = 1.5f;

    [Header("Gravity")]
    public float gravity = -25f;   // Stronger gravity = better game feel

    [Header("Mouse Look")]
    public float mouseSensitivity = 2.0f;
    public Transform cameraRoot;
    public float pitchMin = -90f;
    public float pitchMax = 90f;

    [Header("Cursor Lock")]
    public bool lockCursorOnStart = true;
    public KeyCode unlockKey = KeyCode.Escape;

    CharacterController controller;
    float pitch;
    bool cursorLocked;
    float verticalVelocity;
    bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraRoot == null && Camera.main != null)
            cameraRoot = Camera.main.transform;

        cursorLocked = lockCursorOnStart;
        ApplyCursorState();
    }

    void Update()
    {
        if (Input.GetKeyDown(unlockKey))
        {
            cursorLocked = !cursorLocked;
            ApplyCursorState();
        }

        if (controller == null || cameraRoot == null)
            return;

        // =====================
        // MOUSE LOOK
        // =====================
        if (cursorLocked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

            transform.Rotate(Vector3.up * mouseX);
            cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        // =====================
        // MOVEMENT INPUT
        // =====================
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? moveSpeed * sprintMultiplier
            : moveSpeed;

        Vector3 move = (transform.right * moveX + transform.forward * moveZ).normalized;
        move *= currentSpeed;

        // =====================
        // FLY MODE
        // =====================
        if (flyMode)
        {
            float upDown = 0f;
            if (Input.GetKey(KeyCode.Q)) upDown += 1f;
            if (Input.GetKey(KeyCode.E)) upDown -= 1f;

            verticalVelocity = upDown * verticalSpeed;
        }
        else
        {
            // =====================
            // GROUND CHECK
            // =====================
            isGrounded = controller.isGrounded;

            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f; // Stick to ground
            }

            // =====================
            // JUMP
            // =====================
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // =====================
            // GRAVITY
            // =====================
            verticalVelocity += gravity * Time.deltaTime;

            // Extra gravity when falling (better feel)
            if (verticalVelocity < 0)
            {
                verticalVelocity += gravity * 1.5f * Time.deltaTime;
            }
        }

        // =====================
        // APPLY MOVEMENT
        // =====================
        Vector3 verticalMove = Vector3.up * verticalVelocity;
        controller.Move((move + verticalMove) * Time.deltaTime);
    }

    void ApplyCursorState()
    {
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !cursorLocked;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Water"))
        {
            Debug.Log("Player touched water!");
        }
    }
}
