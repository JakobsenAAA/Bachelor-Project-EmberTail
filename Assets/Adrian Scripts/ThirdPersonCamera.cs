
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

    [Header("Fall Death Camera")]
    [SerializeField] private float fallLookSpeed = 12f;
    [SerializeField] private float fallLookHeight = 0.8f;

    private float yaw;
    private float pitch;
    private bool cameraInputEnabled = true;
    private bool fallDeathActive;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (fallDeathActive)
        {
            UpdateFallDeathCamera();
            return;
        }

        if (cameraInputEnabled && playerController != null)
        {
            Vector2 lookInput = playerController.LookInput;

            yaw += lookInput.x * mouseSensitivity;
            pitch -= lookInput.y * mouseSensitivity;

            pitch = Mathf.Clamp(
                pitch,
                minVerticalAngle,
                maxVerticalAngle
            );
        }

        UpdateCameraPosition(false);
    }

    public void SetCameraInputEnabled(bool enabled)
    {
        cameraInputEnabled = enabled;
    }

    public void StartFallDeathCamera()
    {
        fallDeathActive = true;
    }

    public void StopFallDeathCamera()
    {
        if (!fallDeathActive)
        {
            return;
        }

        fallDeathActive = false;

        if (target == null)
        {
            return;
        }

        Vector3 direction =
            target.position + Vector3.up * height - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);

            yaw = rotation.eulerAngles.y;
            pitch = Mathf.DeltaAngle(0f, rotation.eulerAngles.x);

            pitch = Mathf.Clamp(
                pitch,
                minVerticalAngle,
                maxVerticalAngle
            );
        }
    }

    public void SnapToTarget()
    {
        UpdateCameraPosition(true);
    }

    private void UpdateFallDeathCamera()
    {
        if (target == null)
        {
            return;
        }

        Vector3 lookPosition =
            target.position + Vector3.up * fallLookHeight;

        Vector3 direction = lookPosition - transform.position;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Mathf.Clamp01(fallLookSpeed * Time.deltaTime)
        );
    }

    private void UpdateCameraPosition(bool snapInstantly)
    {
        if (target == null)
        {
            return;
        }

        Quaternion rotation = Quaternion.Euler(
            pitch,
            yaw,
            0f
        );

        Vector3 targetPosition =
            target.position + Vector3.up * height;

        Vector3 desiredDirection =
            -(rotation * Vector3.forward);

        Vector3 desiredPosition =
            targetPosition + desiredDirection * distance;

        if (
            Physics.SphereCast(
                targetPosition,
                collisionRadius,
                desiredDirection,
                out RaycastHit hit,
                distance,
                collisionLayer,
                QueryTriggerInteraction.Ignore
            )
        )
        {
            float finalDistance = Mathf.Clamp(
                hit.distance - collisionOffset,
                minimumDistance,
                distance
            );

            desiredPosition =
                targetPosition + desiredDirection * finalDistance;
        }

        if (snapInstantly)
        {
            transform.position = desiredPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                Mathf.Clamp01(cameraSmoothSpeed * Time.deltaTime)
            );
        }

        transform.LookAt(targetPosition);
    }
}
