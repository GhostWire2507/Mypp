using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Proximity interaction for a computer workstation.
/// Shows an on-screen prompt when the player is close, toggles a "session in use"
/// message when E is pressed and animates the monitor colour while active.
/// </summary>
public class WorkstationInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float interactionRange = 2.5f;
    [SerializeField] private Key interactionKey = Key.E;

    [Header("Monitor")]
    [Tooltip("Renderer of the monitor screen quad. Colour is animated through a MaterialPropertyBlock.")]
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private Color idleColor = new Color(0.05f, 0.09f, 0.12f);
    [SerializeField] private Color activeColor = new Color(0.15f, 0.85f, 0.95f);

    private Transform player;
    private bool playerInRange;
    private bool sessionActive;
    private float messageTimer;
    private MaterialPropertyBlock propertyBlock;
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        ApplyScreenColor(idleColor);
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
            return;
        }

        playerInRange = Vector3.Distance(transform.position, player.position) <= interactionRange;

        if (playerInRange && Keyboard.current != null && Keyboard.current[interactionKey].wasPressedThisFrame)
        {
            sessionActive = !sessionActive;
            messageTimer = 3f;
        }

        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
        }

        UpdateScreen();
    }

    private void UpdateScreen()
    {
        if (sessionActive)
        {
            // Gentle pulse so the active workstation is visibly animated.
            float pulse = 0.65f + Mathf.PingPong(Time.time * 1.6f, 0.35f);
            ApplyScreenColor(Color.Lerp(idleColor, activeColor, pulse));
        }
        else
        {
            ApplyScreenColor(Color.Lerp(idleColor, Color.black, 0.25f));
        }
    }

    private void ApplyScreenColor(Color color)
    {
        if (screenRenderer == null)
        {
            return;
        }

        screenRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(BaseColorId, color);
        screenRenderer.SetPropertyBlock(propertyBlock);
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

        GUIStyle messageStyle = new GUIStyle(labelStyle)
        {
            fontSize = Mathf.RoundToInt(18f * scale),
            normal = { textColor = new Color(0.7f, 1f, 0.85f) }
        };

        float width = 460f * scale;
        float height = 44f * scale;
        Rect promptRect = new Rect((Screen.width - width) * 0.5f, Screen.height - height - 40f, width, height);
        GUI.Box(promptRect, GUIContent.none);
        GUI.Label(promptRect, $"[E] Use computer  ({(sessionActive ? "in use" : "available")})", labelStyle);

        if (messageTimer > 0f)
        {
            Rect messageRect = new Rect((Screen.width - width) * 0.5f, Screen.height - height * 2.2f - 40f, width, height);
            GUI.Box(messageRect, GUIContent.none);
            GUI.Label(messageRect,
                sessionActive ? "Workstation in use - Python lab session active" : "Workstation session ended",
                messageStyle);
        }
    }
}
