using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuCameraMovement : MonoBehaviour
{
    [Header("Movement Limits")]
    [SerializeField] private float maximumLeft = 0.3f;
    [SerializeField] private float maximumRight = 0.3f;
    [SerializeField] private float maximumUp = 0.2f;
    [SerializeField] private float maximumDown = 0.2f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 4f;

    private Vector3 startingPosition;
    private bool movementEnabled = true;

    private void Awake()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        if (!movementEnabled)
        {
            return;
        }

        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        float normalizedMouseX =
            (mousePosition.x / Screen.width) * 2f - 1f;

        float normalizedMouseY =
            (mousePosition.y / Screen.height) * 2f - 1f;

        float horizontalOffset;

        if (normalizedMouseX < 0f)
        {
            horizontalOffset =
                normalizedMouseX *
                maximumLeft;
        }
        else
        {
            horizontalOffset =
                normalizedMouseX *
                maximumRight;
        }

        float verticalOffset;

        if (normalizedMouseY < 0f)
        {
            verticalOffset =
                normalizedMouseY *
                maximumDown;
        }
        else
        {
            verticalOffset =
                normalizedMouseY *
                maximumUp;
        }

        Vector3 targetPosition =
            startingPosition;

        targetPosition +=
            transform.right *
            horizontalOffset;

        targetPosition +=
            transform.up *
            verticalOffset;

        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed *
                Time.unscaledDeltaTime
            );
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }
}