using UnityEngine;

public class BoatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Paddle Transforms")]
    public Transform leftPaddle;
    public Transform rightPaddle;

    [Header("Paddle Rest Rotations")]
    public Vector3 leftPaddleRestRotation  = new Vector3(0f, 0f, -30f);
    public Vector3 rightPaddleRestRotation = new Vector3(0f, 0f,  30f);

    [Header("Boat Rock Settings")]
    public float tiltAngle = 1.5f;
    public float tiltSpeed = 2f;

    [Header("Interaction")]
    public KeyCode boardKey = KeyCode.F;
    public KeyCode moveKey  = KeyCode.W;
    public float   boardingDistance = 3f;

    [Header("Paddle Animation")]
    public float paddleSwingAngle = 60f;
    public float paddleSwingSpeed = 3f;

    [Header("Player Switch")]
    public GameObject playerObject;
    public Camera     playerCamera;
    public Camera     boatCamera;

    // ── private ──────────────────────────────────────────────
    private bool isReady      = false;
    private bool playerInBoat = false;
    private Transform player;

    private float bobTimer    = 0f;
    private float paddleTimer = 0f;
    private bool  isPaddling  = false;
    private float boatYRotation = 0f;

    public void EnableMovement()
    {
        isReady = true;
        Debug.Log("Boat ready! Press F to board.");
    }

    void Start()
    {
        boatYRotation = transform.eulerAngles.y;

        if (playerObject != null)
            player = playerObject.transform;

        if (boatCamera != null) boatCamera.enabled = false;
        ResetPaddles();
    }

    void Update()
    {
        if (!isReady) return;

        HandleBoarding();

        if (playerInBoat)
        {
            HandleMovement();
            AnimatePaddles();
        }
        else
        {
            isPaddling = false;
            ReturnPaddlesToRest();
        }

        // Gentle rock
        bobTimer += Time.deltaTime * tiltSpeed;
        float tilt = Mathf.Sin(bobTimer) * tiltAngle;
        transform.rotation = Quaternion.Euler(0f, boatYRotation, tilt);
    }

    void HandleMovement()
    {
        isPaddling = Input.GetKey(moveKey);
        if (!isPaddling) return;

        Vector3 forward = Quaternion.Euler(0f, boatYRotation, 0f) * Vector3.forward;
        Vector3 nextPos = transform.position + forward * moveSpeed * Time.deltaTime;

        // Cast a box in front of the boat to detect collisions before moving
        Vector3 boxSize = new Vector3(1.5f, 0.5f, 0.5f);
        bool blocked = Physics.BoxCast(
            transform.position,
            boxSize,
            forward,
            Quaternion.identity,
            moveSpeed * Time.deltaTime + 0.5f
        );

        if (!blocked)
            transform.position = nextPos;
        else
            Debug.Log("Boat blocked by obstacle!");
    }

    void HandleBoarding()
    {
        if (!Input.GetKeyDown(boardKey)) return;

        if (!playerInBoat)
        {
            if (player == null) return;
            float dist = Vector3.Distance(player.position, transform.position);
            if (dist <= boardingDistance)
                BoardBoat();
            else
                Debug.Log("Too far from boat!");
        }
        else
        {
            ExitBoat();
        }
    }

    void BoardBoat()
    {
        playerInBoat = true;
        playerObject.SetActive(false);
        if (playerCamera != null) playerCamera.enabled = false;
        if (boatCamera   != null) boatCamera.enabled   = true;
        Debug.Log("Boarded! Hold W to move. Press F to exit.");
    }

    void ExitBoat()
    {
        playerInBoat = false;
        player.position = transform.position + transform.right * 2f + Vector3.up * 0.5f;
        playerObject.SetActive(true);
        if (playerCamera != null) playerCamera.enabled = true;
        if (boatCamera   != null) boatCamera.enabled   = false;
        Debug.Log("Exited boat.");
    }

    void AnimatePaddles()
    {
        if (isPaddling)
        {
            paddleTimer += Time.deltaTime * paddleSwingSpeed;

            if (leftPaddle)
            {
                Vector3 r = leftPaddleRestRotation;
                r.x += Mathf.Sin(paddleTimer) * paddleSwingAngle;
                leftPaddle.localEulerAngles = r;
            }

            if (rightPaddle)
            {
                Vector3 r = rightPaddleRestRotation;
                r.x += Mathf.Sin(paddleTimer + Mathf.PI) * paddleSwingAngle;
                rightPaddle.localEulerAngles = r;
            }
        }
        else
        {
            ReturnPaddlesToRest();
        }
    }

    void ReturnPaddlesToRest()
    {
        paddleTimer = 0f;
        if (leftPaddle)
            leftPaddle.localRotation = Quaternion.Slerp(
                leftPaddle.localRotation,
                Quaternion.Euler(leftPaddleRestRotation),
                Time.deltaTime * 5f);
        if (rightPaddle)
            rightPaddle.localRotation = Quaternion.Slerp(
                rightPaddle.localRotation,
                Quaternion.Euler(rightPaddleRestRotation),
                Time.deltaTime * 5f);
    }

    void ResetPaddles()
    {
        if (leftPaddle)  leftPaddle.localEulerAngles = leftPaddleRestRotation;
        if (rightPaddle) rightPaddle.localEulerAngles = rightPaddleRestRotation;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isReady ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, boardingDistance);
    }
}