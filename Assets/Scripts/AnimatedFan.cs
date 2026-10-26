using UnityEngine;

/// <summary>
/// Continuously rotates an object, used here for the classroom ceiling fan.
/// The rotation speed can be slowed down or paused from the Inspector.
/// </summary>
public class AnimatedFan : MonoBehaviour
{
    [SerializeField] private Vector3 localRotationAxis = Vector3.up;
    [SerializeField] private float degreesPerSecond = 240f;

    private void Update()
    {
        transform.Rotate(localRotationAxis, degreesPerSecond * Time.deltaTime, Space.Self);
    }
}
