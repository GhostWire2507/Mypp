using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorController : MonoBehaviour
{
    [Header("Door")]
    [Tooltip("Pivot GameObject placed on the door hinge.")]
    [SerializeField] private Transform hinge;
    [SerializeField] private float openAngle = -100f;
    [SerializeField] private float openCloseDuration = 1f;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float interactionRange = 2.6f;
    [SerializeField] private Key interactionKey = Key.F;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine rotationRoutine;
    private Transform player;
    private bool isOpen;
    private bool playerInRange;

    private void Awake()
    {
        if (hinge == null)
        {
            hinge = transform.parent;
        }

        if (hinge == null)
        {
            Debug.LogError("DoorController: no hinge assigned.", this);
            enabled = false;
            return;
        }

        closedRotation = hinge.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            if (player == null)
            {
                playerInRange = false;
                return;
            }
        }

        Vector3 doorPosition = transform.position;
        Vector3 playerPosition = player.position;
        float offsetX = doorPosition.x - playerPosition.x;
        float offsetZ = doorPosition.z - playerPosition.z;
        playerInRange = offsetX * offsetX + offsetZ * offsetZ <= interactionRange * interactionRange;

        if (playerInRange && Keyboard.current != null
            && Keyboard.current[interactionKey].wasPressedThisFrame)
        {
            SetOpen(!isOpen);
        }
    }

    private void SetOpen(bool open)
    {
        isOpen = open;
        StartRotation(isOpen ? openRotation : closedRotation);
    }

    private void StartRotation(Quaternion targetRotation)
    {
        if (rotationRoutine != null)
        {
            StopCoroutine(rotationRoutine);
        }

        rotationRoutine = StartCoroutine(RotateHinge(targetRotation));
    }

    private IEnumerator RotateHinge(Quaternion targetRotation)
    {
        Quaternion startRotation = hinge.localRotation;
        float elapsed = 0f;

        while (elapsed < openCloseDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / openCloseDuration));
            hinge.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        hinge.localRotation = targetRotation;
        rotationRoutine = null;
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void OnGUI()
    {
        if (!playerInRange)
        {
            return;
        }

        float scale = Mathf.Max(1f, Screen.height / 900f);
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.RoundToInt(20f * scale),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        float width = 360f * scale;
        float height = 44f * scale;
        Rect promptRect = new Rect((Screen.width - width) * 0.5f, Screen.height - height - 40f, width, height);
        GUI.Box(promptRect, GUIContent.none);
        GUI.Label(promptRect, $"[F] {(isOpen ? "Close" : "Open")} door", labelStyle);
    }
}
