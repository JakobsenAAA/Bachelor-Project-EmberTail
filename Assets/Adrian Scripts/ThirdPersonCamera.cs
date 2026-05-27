using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private PlayerController playerController;

    [Header("Camera")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 0f;
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float minVerticalAngle = -25f;
    [SerializeField] private float maxVerticalAngle = 65f;
    [SerializeField] private float cameraSmoothSpeed = 15f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private float collisionRadius = 0.25f;
    [SerializeField] private float collisionOffset = 0.15f;
    [SerializeField] private float minimumDistance = 0.75f;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        Vector2 lookInput = playerController.LookInput;

        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = target.position + Vector3.up * height;
        Vector3 desiredDirection = -(rotation * Vector3.forward);
        Vector3 desiredPosition = targetPosition + desiredDirection * distance;

        float finalDistance = distance;

        if (Physics.SphereCast(targetPosition, collisionRadius, desiredDirection, out RaycastHit hit, distance, collisionLayer, QueryTriggerInteraction.Ignore))
        {
            finalDistance = Mathf.Clamp(hit.distance - collisionOffset, minimumDistance, distance);
            desiredPosition = targetPosition + desiredDirection * finalDistance;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, cameraSmoothSpeed * Time.deltaTime);
        transform.LookAt(targetPosition);
    }
}