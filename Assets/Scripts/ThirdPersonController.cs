using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Camera-relative third-person character movement built on CharacterController.
/// Handles WASD movement, smooth turning, gravity and ground contact.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camera used to make WASD movement relative to the view.")]
    [SerializeField] private Transform cameraTransform;
    [Tooltip("Animator driven by the character's idle, walk and stair clips.")]
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4.5f;
    [Tooltip("Time in seconds used to smooth character turning.")]
    [SerializeField] private float turnSmoothTime = 0.08f;
    [SerializeField] private float gravity = -22f;
    [Tooltip("Small downward force that keeps the controller grounded on slopes and steps.")]
    [SerializeField] private float groundedStickForce = -2f;

    [Header("Stair animation")]
    [Tooltip("Centre of the world-space box that marks the straight stair and its top landing.")]
    [SerializeField] private Vector3 stairZoneCenter = new Vector3(2.525f, 1.825f, 6.45f);
    [Tooltip("Size of the world-space box that marks the straight stair and its top landing.")]
    [SerializeField] private Vector3 stairZoneSize = new Vector3(4.45f, 3.45f, 1.26f);
    [Tooltip("World direction that points uphill along the stair.")]
    [SerializeField] private Vector3 stairUpDirection = Vector3.right;
    [Tooltip("Minimum movement along the stair axis before it counts as climbing or descending.")]
    [SerializeField] private float stairDirectionThreshold = 0.2f;

    private CharacterController characterController;
    private float verticalVelocity;
    private float turnVelocity;

    /// <summary>True while the player is inside the stair volume.</summary>
    public bool IsOnStairs { get; private set; }

    /// <summary>1 while climbing, -1 while descending, 0 otherwise.</summary>
    public int StairMode { get; private set; }

    /// <summary>Horizontal speed reported to the Animator.</summary>
    public float PlanarSpeed { get; private set; }

    /// <summary>Raw WASD input for the current frame.</summary>
    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            return;
        }

        MoveInput = Vector2.zero;
        PlanarSpeed = 0f;
        StairMode = 0;
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetInteger("StairMode", 0);
        }
    }

    private void Update()
    {
        if (!Application.isFocused)
        {
            MoveInput = Vector2.zero;
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
                animator.SetInteger("StairMode", 0);
            }
            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            MoveInput = Vector2.zero;
            return;
        }

        Vector2 input = Vector2.zero;
        if (keyboard.wKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed) input.y -= 1f;
        if (keyboard.dKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed) input.x -= 1f;
        MoveInput = input;

        Vector3 moveDirection = Vector3.zero;

        if (input.sqrMagnitude > 0.01f && cameraTransform != null)
        {
            // Flatten the camera axes so movement stays on the ground plane.
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            moveDirection = (forward * input.y + right * input.x).normalized;

            // Rotate the character towards the movement direction.
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetAngle, ref turnVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
        }

        // Apply gravity, with a small downward bias while grounded.
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedStickForce;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;
        characterController.Move(velocity * Time.deltaTime);

        UpdateAnimation(moveDirection);
    }

    private void UpdateAnimation(Vector3 moveDirection)
    {
        if (animator == null)
        {
            return;
        }

        PlanarSpeed = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z).magnitude;

        Vector3 offset = transform.position - stairZoneCenter;
        IsOnStairs =
            Mathf.Abs(offset.x) <= stairZoneSize.x * 0.5f &&
            Mathf.Abs(offset.y) <= stairZoneSize.y * 0.5f &&
            Mathf.Abs(offset.z) <= stairZoneSize.z * 0.5f;

        int stairMode = 0;
        if (IsOnStairs && moveDirection.sqrMagnitude > 0.01f)
        {
            float alongStair = Vector3.Dot(moveDirection, stairUpDirection.normalized);
            if (alongStair > stairDirectionThreshold)
            {
                stairMode = 1;
            }
            else if (alongStair < -stairDirectionThreshold)
            {
                stairMode = -1;
            }
        }

        StairMode = stairMode;
        animator.SetFloat("Speed", PlanarSpeed);
        animator.SetInteger("StairMode", stairMode);
    }
}
