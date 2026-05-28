using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerSlopeSlide : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField] private LayerMask slideLayer;
    [SerializeField] private LayerMask normalGroundLayer;

    [Header("Detection")]
    [SerializeField] private float slopeAngleThreshold = 25f;
    [SerializeField] private float groundProbeRadius = 0.3f;
    [SerializeField] private float groundProbeDistance = 1.2f;
    [SerializeField] private float probeStartHeight = 0.5f;
    [SerializeField] private float contactMemoryTime = 0.12f;

    [Header("Slide Movement")]
    [SerializeField] private float slideAcceleration = 18f;
    [SerializeField] private float maxSlideSpeed = 12f;
    [SerializeField] private float slideStickForce = 6f;
    [SerializeField] private float steeringStrength = 2.5f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Slide Jump")]
    [SerializeField] private float slideJumpHeight = 2.4f;
    [SerializeField] private float slideJumpForwardSpeed = 10f;
    [SerializeField] private float slideJumpGravity = -25f;
    [SerializeField] private float slideJumpMinAirTime = 0.12f;

    private CharacterController characterController;
    private Vector3 slideVelocity;
    private Vector3 slideJumpVelocity;
    private Vector3 lastSlideContactNormal;
    private float lastSlideContactTime;
    private float slideJumpTimer;
    private bool isSlideJumping;

    public bool IsSliding { get; private set; }
    public bool CanUseNormalMovement => !IsSliding && !isSlideJumping;
    public Vector3 CurrentSlideDirection { get; private set; }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!IsInLayerMask(hit.collider.gameObject.layer, slideLayer))
        {
            return;
        }

        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

        if (slopeAngle >= slopeAngleThreshold && slopeAngle < 90f)
        {
            lastSlideContactNormal = hit.normal;
            lastSlideContactTime = Time.time;
        }
    }

    public bool Tick(Vector2 moveInput, Transform cameraTransform, bool jumpPressed)
    {
        if (isSlideJumping)
        {
            HandleSlideJump();
            return true;
        }

        bool hasSlideSurface = TryGetSlideSurface(out Vector3 slideNormal);

        if (!hasSlideSurface)
        {
            StopSliding();
            return false;
        }

        IsSliding = true;

        Vector3 downhillDirection = Vector3.ProjectOnPlane(Vector3.down, slideNormal).normalized;

        if (CurrentSlideDirection.sqrMagnitude < 0.01f)
        {
            CurrentSlideDirection = downhillDirection;
        }

        Vector3 steeringDirection = GetSteeringDirection(moveInput, cameraTransform, slideNormal);
        Vector3 desiredDirection = downhillDirection;

        if (steeringDirection.sqrMagnitude > 0.01f)
        {
            desiredDirection = Vector3.Slerp(downhillDirection, steeringDirection, steeringStrength * 0.1f).normalized;
            desiredDirection = Vector3.ProjectOnPlane(desiredDirection, slideNormal).normalized;
        }

        CurrentSlideDirection = Vector3.Slerp(CurrentSlideDirection, desiredDirection, rotationSpeed * Time.deltaTime).normalized;

        if (jumpPressed)
        {
            StartSlideJump();
            return true;
        }

        slideVelocity += CurrentSlideDirection * slideAcceleration * Time.deltaTime;
        slideVelocity = Vector3.ClampMagnitude(slideVelocity, maxSlideSpeed);

        Vector3 movement = slideVelocity + Vector3.down * slideStickForce;
        characterController.Move(movement * Time.deltaTime);

        if (CurrentSlideDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(CurrentSlideDirection.x, 0f, CurrentSlideDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        return true;
    }

    private bool TryGetSlideSurface(out Vector3 slideNormal)
    {
        slideNormal = Vector3.up;

        if (TryGetGround(out RaycastHit groundHit))
        {
            bool onSlideLayer = IsInLayerMask(groundHit.collider.gameObject.layer, slideLayer);
            bool onNormalLayer = IsInLayerMask(groundHit.collider.gameObject.layer, normalGroundLayer);
            float slopeAngle = Vector3.Angle(groundHit.normal, Vector3.up);

            if (onNormalLayer && !onSlideLayer)
            {
                return false;
            }

            if (onSlideLayer && slopeAngle >= slopeAngleThreshold && slopeAngle < 90f)
            {
                slideNormal = groundHit.normal;
                return true;
            }
        }

        if (Time.time - lastSlideContactTime <= contactMemoryTime)
        {
            float contactAngle = Vector3.Angle(lastSlideContactNormal, Vector3.up);

            if (contactAngle >= slopeAngleThreshold && contactAngle < 90f)
            {
                slideNormal = lastSlideContactNormal;
                return true;
            }
        }

        return false;
    }

    private void HandleSlideJump()
    {
        slideJumpTimer += Time.deltaTime;
        slideJumpVelocity.y += slideJumpGravity * Time.deltaTime;
        characterController.Move(slideJumpVelocity * Time.deltaTime);

        if (slideJumpTimer >= slideJumpMinAirTime && characterController.isGrounded && slideJumpVelocity.y <= 0f)
        {
            isSlideJumping = false;
            slideJumpVelocity = Vector3.zero;
            slideVelocity = Vector3.zero;

            if (TryGetSlideSurface(out Vector3 slideNormal))
            {
                IsSliding = true;
                CurrentSlideDirection = Vector3.ProjectOnPlane(Vector3.down, slideNormal).normalized;
                return;
            }

            StopSliding();
        }
    }

    private void StartSlideJump()
    {
        IsSliding = false;
        isSlideJumping = true;
        slideJumpTimer = 0f;

        Vector3 jumpDirection = CurrentSlideDirection;

        if (jumpDirection.sqrMagnitude < 0.01f)
        {
            jumpDirection = transform.forward;
        }

        jumpDirection.y = 0f;
        jumpDirection.Normalize();

        float upwardVelocity = Mathf.Sqrt(slideJumpHeight * -2f * slideJumpGravity);
        slideJumpVelocity = jumpDirection * slideJumpForwardSpeed;
        slideJumpVelocity.y = upwardVelocity;
        slideVelocity = Vector3.zero;
    }

    private void StopSliding()
    {
        IsSliding = false;
        isSlideJumping = false;
        slideVelocity = Vector3.zero;
        slideJumpVelocity = Vector3.zero;
        slideJumpTimer = 0f;
        CurrentSlideDirection = Vector3.zero;
    }

    private bool TryGetGround(out RaycastHit hit)
    {
        Vector3 origin = transform.position + Vector3.up * probeStartHeight;
        LayerMask combinedLayer = slideLayer | normalGroundLayer;
        return Physics.SphereCast(origin, groundProbeRadius, Vector3.down, out hit, groundProbeDistance, combinedLayer, QueryTriggerInteraction.Ignore);
    }

    private Vector3 GetSteeringDirection(Vector2 moveInput, Transform cameraTransform, Vector3 groundNormal)
    {
        if (cameraTransform == null || moveInput.sqrMagnitude < 0.01f)
        {
            return Vector3.zero;
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 steeringDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

        if (steeringDirection.sqrMagnitude > 1f)
        {
            steeringDirection.Normalize();
        }

        return Vector3.ProjectOnPlane(steeringDirection, groundNormal).normalized;
    }

    private bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return (layerMask.value & (1 << layer)) != 0;
    }
}