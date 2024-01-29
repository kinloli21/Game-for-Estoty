using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float moveInputDeadZone;

    private int leftFingerId, rightFingerId;
    private float halfScreenWidth;

    private Vector2 lookInput;
    private float cameraPitch;

    private Vector2 moveTouchStartPosition;
    private Vector2 moveInput;

    // Gravity variables
    private float verticalVelocity = 0f;
    [SerializeField] private float gravity = -9.81f;

    void Start()
    {
        leftFingerId = -1;
        rightFingerId = -1;

        halfScreenWidth = Screen.width / 2;
        moveInputDeadZone = Mathf.Pow(Screen.height / moveInputDeadZone, 2);
    }

    void Update()
    {
        GetTouchInput();

        if (rightFingerId != -1)
        {
            Debug.Log("Rotating");
            LookAround();
        }

        if (leftFingerId != -1)
        {
            Debug.Log("Moving");
            Move();
        }

        // Apply gravity
        ApplyGravity();
    }

    void GetTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (t.position.x < halfScreenWidth && leftFingerId == -1)
                    {
                        leftFingerId = t.fingerId;
                        moveTouchStartPosition = t.position;
                    }
                    else if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        rightFingerId = t.fingerId;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == leftFingerId)
                    {
                        leftFingerId = -1;
                        Debug.Log("Stopped tracking left finger");
                    }
                    else if (t.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                        Debug.Log("Stopped tracking right finger");
                    }
                    break;

                case TouchPhase.Moved:
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    else if (t.fingerId == leftFingerId)
                    {
                        moveInput = t.position - moveTouchStartPosition;
                    }
                    break;

                case TouchPhase.Stationary:
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    void LookAround()
    {
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(transform.up, lookInput.x);
    }

    void Move()
    {
        if (moveInput.sqrMagnitude <= moveInputDeadZone) return;

        Vector2 movementDirection = moveInput.normalized * moveSpeed * Time.deltaTime;
        characterController.Move(transform.right * movementDirection.x + transform.forward * movementDirection.y);
    }

    void ApplyGravity()
    {
        // Check if the character controller is grounded
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f; // A small value to ensure snapping to the ground
        }
        else
        {
            // Apply gravity
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Apply the vertical velocity to the character controller
        Vector3 gravityVector = new Vector3(0, verticalVelocity, 0);
        characterController.Move(gravityVector * Time.deltaTime);
    }
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a GameObject with a specific tag
        if (other.CompareTag("Transition"))
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
