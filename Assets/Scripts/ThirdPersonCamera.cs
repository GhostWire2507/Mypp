using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple orbit camera for a third-person character.
/// Mouse moves the camera, Escape releases the cursor and a mouse click captures it again.
/// A sphere cast keeps the camera from clipping through walls.
/// </summary>
public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 pivotOffset = new Vector3(0f, 1.55f, 0f);

    [Header("Orbit")]
    [SerializeField] private float distance = 4.5f;
    [SerializeField] private float indoorDistance = 2.8f;
    [SerializeField] private float distanceLerpSpeed = 6f;
    [SerializeField] private float mouseSensitivity = 1.1f;
    [SerializeField] private float minPitch = -15f;
    [SerializeField] private float maxPitch = 65f;
    [SerializeField] private float followSmoothTime = 0.08f;

    [Header("Auto Align")]
    [SerializeField] private float autoAlignDelay = 0.35f;
    [SerializeField] private float autoAlignSpeed = 10f;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private float collisionPadding = 0.2f;
    [SerializeField] private LayerMask collisionMask = ~0;

    private float yaw;
    private float pitch = 18f;
    private Vector3 followVelocity;
    private float currentDistance;
    private float lastMouseInputTime = -10f;
    private bool skipNextMouseDelta;
    private CharacterController characterController;
    private ThirdPersonController playerController;

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        if (target != null)
        {
            yaw = target.eulerAngles.y;
            characterController = target.GetComponent<CharacterController>();
            playerController = target.GetComponent<ThirdPersonController>();
        }

        currentDistance = distance;
        LockCursor(true);
        skipNextMouseDelta = true;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        HandleCursorAndLook();

        bool moving = false;
        if (characterController != null)
        {
            Vector3 planarVelocity = characterController.velocity;
            planarVelocity.y = 0f;
            moving = planarVelocity.sqrMagnitude > 0.04f;
        }

        bool walkingForward = playerController == null
            || (playerController.MoveInput.y > 0.1f
                && Mathf.Abs(playerController.MoveInput.x) < 0.35f);

        if (moving && walkingForward && Time.time - lastMouseInputTime > autoAlignDelay)
        {
            float targetYaw = target.eulerAngles.y;
            yaw = Mathf.LerpAngle(yaw, targetYaw, 1f - Mathf.Exp(-autoAlignSpeed * Time.deltaTime));
        }

        float targetDistance = IsInsideBuilding(target.position) ? indoorDistance : distance;
        currentDistance = Mathf.Lerp(
            currentDistance, targetDistance, 1f - Mathf.Exp(-distanceLerpSpeed * Time.deltaTime));

        Vector3 pivot = target.position + pivotOffset;
        Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 cameraDirection = orbitRotation * Vector3.back;
        float desiredDistance = currentDistance;

        // Pull the camera in if a wall is in the way.
        if (Physics.SphereCast(
                pivot,
                collisionRadius,
                cameraDirection,
                out RaycastHit hit,
                currentDistance,
                collisionMask,
                QueryTriggerInteraction.Ignore))
        {
            if (!hit.collider.transform.IsChildOf(target))
            {
                desiredDistance = Mathf.Max(0.6f, hit.distance - collisionPadding);
            }
        }

        Vector3 desiredPosition = pivot + cameraDirection * desiredDistance;
        desiredPosition.y = Mathf.Max(desiredPosition.y, 0.35f);

        transform.position = Vector3.SmoothDamp(
            transform.position, desiredPosition, ref followVelocity, followSmoothTime);

        Vector3 lookDirection = pivot - transform.position;
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }

    private void HandleCursorAndLook()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            LockCursor(false);
        }
        else if (mouse != null &&
                 mouse.leftButton.wasPressedThisFrame &&
                 Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor(true);
            skipNextMouseDelta = true;
        }

        if (mouse == null || Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        if (skipNextMouseDelta)
        {
            skipNextMouseDelta = false;
            return;
        }

        Vector2 delta = mouse.delta.ReadValue();
        if (delta.sqrMagnitude > 0.0001f)
        {
            lastMouseInputTime = Time.time;
        }

        yaw += delta.x * mouseSensitivity;
        pitch = Mathf.Clamp(pitch - delta.y * mouseSensitivity, minPitch, maxPitch);
    }

    private static bool IsInsideBuilding(Vector3 position)
    {
        return position.x > -5.9f && position.x < 7.3f
            && position.z > -4.1f && position.z < 3.9f;
    }

    private static void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    private void OnDisable()
    {
        LockCursor(false);
    }
}
