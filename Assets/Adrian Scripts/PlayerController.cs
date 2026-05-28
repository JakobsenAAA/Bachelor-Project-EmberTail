using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject dashAttackBox;
    [SerializeField] private GameObject spinAttackBox;
    [SerializeField] private GameObject groundSlamAttackBox;
    [SerializeField] private GameObject uppercutAttackBox;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float doubleJumpHeight = 1.6f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private bool sprintOnlyWhileGrounded = true;
    [SerializeField] private bool allowDoubleJump = true;

    [Header("Slide")]
    [SerializeField] private LayerMask slideLayer;
    [SerializeField] private float maxStableSlopeAngle = 25f;
    [SerializeField] private float slideAcceleration = 18f;
    [SerializeField] private float maxSlideSpeed = 12f;
    [SerializeField] private float slideSteeringStrength = 4f;
    [SerializeField] private float slideRaycastDistance = 1.4f;
    [SerializeField] private float slideStickToGroundForce = 5f;

    [Header("Crawl")]
    [SerializeField] private float crawlSpeed = 3f;
    [SerializeField] private float crawlHeight = 0.9f;
    [SerializeField] private float crawlHoldTimeForHighJump = 1f;
    [SerializeField] private float crawlHighJumpHeight = 3.5f;
    [SerializeField] private bool preventStandingWhenBlocked = true;
    [SerializeField] private float standCheckRadius = 0.25f;
    [SerializeField] private float crawlGroundedGraceTime = 0.15f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashAttackExtraTime = 0.08f;

    [Header("Spin")]
    [SerializeField] private float spinDuration = 0.35f;
    [SerializeField] private float airSpinGravityMultiplier = 0.25f;
    [SerializeField] private float airSpinMaxFallSpeed = 3f;

    [Header("Ground Slam")]
    [SerializeField] private float groundSlamHoverTime = 0.5f;
    [SerializeField] private float groundSlamGravity = -80f;
    [SerializeField] private float groundSlamImpactTime = 0.15f;
    [SerializeField] private bool allowGroundSlamAfterDashOrSpin = true;

    [Header("Uppercut")]
    [SerializeField] private float uppercutInputBufferTime = 0.18f;
    [SerializeField] private float uppercutHoverTime = 0.2f;
    [SerializeField] private float uppercutJumpHeight = 3.5f;
    [SerializeField] private float uppercutAttackTime = 0.25f;

    [Header("Special Action Limit")]
    [SerializeField] private bool limitAirSpecialActions = true;
    [SerializeField] private float sharedSpecialActionCooldown = 0.5f;
    [SerializeField] private bool resetSpecialActionCooldownOnJump = true;

    [Header("Grounding")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.35f;
    [SerializeField] private LayerMask groundLayer;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private Vector3 currentMoveDirection;
    private Vector3 currentGroundNormal = Vector3.up;
    private Vector3 slideVelocity;
    private Coroutine sharedCooldownCoroutine;
    private float standingHeight;
    private Vector3 standingCenter;
    private float standingBottomY;
    private float crawlTimer;
    private float uppercutBufferTimer;
    private float lastGroundedTime;
    private bool jumpPressed;
    private bool uppercutRequested;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isDashing;
    private bool isSpinning;
    private bool isGroundSlamming;
    private bool isUppercutHovering;
    private bool isUppercutMovementLocked;
    private bool isCrawling;
    private bool isSliding;
    private bool canDoubleJump;
    private bool canUseAirDashOrSpin = true;
    private bool canUseSharedDashSpinAction = true;
    private bool hasUsedGroundSlam;
    private bool airActionsLockedUntilGrounded;

    public Vector2 LookInput => lookInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        standingHeight = characterController.height;
        standingCenter = characterController.center;
        standingBottomY = standingCenter.y - standingHeight * 0.5f;

        if (dashAttackBox != null)
        {
            dashAttackBox.SetActive(false);
        }

        if (spinAttackBox != null)
        {
            spinAttackBox.SetActive(false);
        }

        if (groundSlamAttackBox != null)
        {
            groundSlamAttackBox.SetActive(false);
        }

        if (uppercutAttackBox != null)
        {
            uppercutAttackBox.SetActive(false);
        }
    }

    private void Update()
    {
        CheckGrounded();
        ResetAirActionsOnLanding();
        HandleUppercutBuffer();
        HandleSlideState();
        HandleCrawlState();
        ApplyCrawlSize();

        if (isSliding && !isDashing && !isGroundSlamming && !isUppercutHovering && !isUppercutMovementLocked)
        {
            HandleSlideMovement();
            HandleJump();
        }
        else if (!isDashing && !isGroundSlamming && !isUppercutHovering && !isUppercutMovementLocked)
        {
            HandleMovement();
            HandleJump();
        }
        else if (!isDashing && !isGroundSlamming && !isUppercutHovering)
        {
            HandleJump();
        }

        if (!isGroundSlamming && !isUppercutHovering)
        {
            HandleGravity();
        }

        wasGrounded = isGrounded;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (isDashing || uppercutBufferTimer > 0f)
            {
                uppercutRequested = true;
            }
            else
            {
                jumpPressed = true;
            }
        }
    }

    public void OnSprint(InputValue value)
    {
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && CanStartDashOrSpin())
        {
            StartCoroutine(Dash());
        }
    }

    public void OnSpin(InputValue value)
    {
        if (value.isPressed && CanStartDashOrSpin())
        {
            StartCoroutine(Spin());
        }
    }

    public void OnGroundSlam(InputValue value)
    {
        if (value.isPressed && CanStartGroundSlam())
        {
            StartCoroutine(GroundSlam());
        }
    }

    private void CheckGrounded()
    {
        LayerMask groundingLayers = groundLayer | slideLayer;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundingLayers, QueryTriggerInteraction.Ignore);

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, slideRaycastDistance, groundingLayers, QueryTriggerInteraction.Ignore))
        {
            currentGroundNormal = hit.normal;
        }
        else
        {
            currentGroundNormal = Vector3.up;
        }

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (isGrounded && velocity.y < 0f && !isGroundSlamming)
        {
            velocity.y = -2f;
        }
    }

    private void ResetAirActionsOnLanding()
    {
        if (!wasGrounded && isGrounded)
        {
            canDoubleJump = true;
            canUseAirDashOrSpin = true;
            hasUsedGroundSlam = false;
            airActionsLockedUntilGrounded = false;
            isUppercutMovementLocked = false;
        }

        if (isGrounded && !isDashing && !isSpinning && !isGroundSlamming && !isUppercutHovering)
        {
            canDoubleJump = true;
            canUseAirDashOrSpin = true;
            hasUsedGroundSlam = false;
            airActionsLockedUntilGrounded = false;
            isUppercutMovementLocked = false;
        }
    }

    private void HandleUppercutBuffer()
    {
        if (uppercutBufferTimer > 0f)
        {
            uppercutBufferTimer -= Time.deltaTime;
        }
    }

    private void HandleSlideState()
    {
        bool shouldSlide = false;
        LayerMask groundingLayers = groundLayer | slideLayer;

        if (isGrounded && Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, slideRaycastDistance, groundingLayers, QueryTriggerInteraction.Ignore))
        {
            bool isSlideSurface = ((1 << hit.collider.gameObject.layer) & slideLayer.value) != 0;
            float groundAngle = Vector3.Angle(hit.normal, Vector3.up);
            shouldSlide = isSlideSurface && groundAngle > maxStableSlopeAngle;
            currentGroundNormal = hit.normal;
        }

        if (shouldSlide)
        {
            isSliding = true;
            isCrawling = false;
            crawlTimer = 0f;
            return;
        }

        isSliding = false;
        slideVelocity = Vector3.MoveTowards(slideVelocity, Vector3.zero, slideAcceleration * Time.deltaTime);
    }

    private void HandleCrawlState()
    {
        bool crawlHeld = Keyboard.current != null && Keyboard.current.leftCtrlKey.isPressed;
        bool recentlyGrounded = Time.time - lastGroundedTime <= crawlGroundedGraceTime;
        bool wantsToCrawl = crawlHeld && recentlyGrounded && !isSliding && !isDashing && !isSpinning && !isGroundSlamming && !isUppercutHovering && !isUppercutMovementLocked;

        if (wantsToCrawl)
        {
            isCrawling = true;
            crawlTimer += Time.deltaTime;
            return;
        }

        if (!preventStandingWhenBlocked || CanStandUp())
        {
            isCrawling = false;
            crawlTimer = 0f;
        }
    }

    private void ApplyCrawlSize()
    {
        if (isCrawling)
        {
            characterController.height = crawlHeight;
            characterController.center = new Vector3(standingCenter.x, standingBottomY + crawlHeight * 0.5f, standingCenter.z);
        }
        else
        {
            characterController.height = standingHeight;
            characterController.center = standingCenter;
        }
    }

    private bool CanStandUp()
    {
        LayerMask obstructionLayers = groundLayer | slideLayer;
        Vector3 checkPosition = transform.position + standingCenter + Vector3.up * ((standingHeight * 0.5f) - standCheckRadius);
        return !Physics.CheckSphere(checkPosition, standCheckRadius, obstructionLayers, QueryTriggerInteraction.Ignore);
    }

    private bool CanStartDashOrSpin()
    {
        if (isSliding)
        {
            return false;
        }

        if (airActionsLockedUntilGrounded)
        {
            return false;
        }

        if (!canUseSharedDashSpinAction)
        {
            return false;
        }

        if (isDashing || isSpinning || isGroundSlamming || isUppercutHovering || isUppercutMovementLocked)
        {
            return false;
        }

        if (limitAirSpecialActions && !isGrounded && !canUseAirDashOrSpin)
        {
            return false;
        }

        return true;
    }

    private bool CanStartGroundSlam()
    {
        if (isSliding)
        {
            return false;
        }

        if (airActionsLockedUntilGrounded)
        {
            return false;
        }

        if (isGrounded)
        {
            return false;
        }

        if (isDashing || isSpinning || isGroundSlamming || isUppercutHovering || isUppercutMovementLocked)
        {
            return false;
        }

        if (hasUsedGroundSlam)
        {
            return false;
        }

        if (!allowGroundSlamAfterDashOrSpin && limitAirSpecialActions && !canUseAirDashOrSpin)
        {
            return false;
        }

        return true;
    }

    private void UseAirDashOrSpin()
    {
        canUseSharedDashSpinAction = false;

        if (sharedCooldownCoroutine != null)
        {
            StopCoroutine(sharedCooldownCoroutine);
        }

        if (limitAirSpecialActions && !isGrounded)
        {
            canUseAirDashOrSpin = false;
            canDoubleJump = false;
        }
    }

    private void UseGroundSlam()
    {
        hasUsedGroundSlam = true;
        canUseAirDashOrSpin = false;
        canDoubleJump = false;
    }

    private void LockAirActionsUntilGrounded()
    {
        airActionsLockedUntilGrounded = true;
        canDoubleJump = false;
        canUseAirDashOrSpin = false;
        hasUsedGroundSlam = true;
    }

    private void ResetSharedSpecialActionCooldown()
    {
        if (sharedCooldownCoroutine != null)
        {
            StopCoroutine(sharedCooldownCoroutine);
            sharedCooldownCoroutine = null;
        }

        canUseSharedDashSpinAction = true;
    }

    private void StartSharedSpecialActionCooldown()
    {
        if (sharedCooldownCoroutine != null)
        {
            StopCoroutine(sharedCooldownCoroutine);
        }

        sharedCooldownCoroutine = StartCoroutine(SharedSpecialActionCooldown());
    }

    private IEnumerator SharedSpecialActionCooldown()
    {
        yield return new WaitForSeconds(sharedSpecialActionCooldown);
        canUseSharedDashSpinAction = true;
        sharedCooldownCoroutine = null;
    }

    private void HandleMovement()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        currentMoveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

        if (currentMoveDirection.sqrMagnitude > 1f)
        {
            currentMoveDirection.Normalize();
        }

        bool sprintHeld = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        bool canSprint = sprintHeld && !isCrawling && (!sprintOnlyWhileGrounded || isGrounded);
        float currentSpeed = isCrawling ? crawlSpeed : canSprint ? sprintSpeed : moveSpeed;

        characterController.Move(currentMoveDirection * currentSpeed * Time.deltaTime);

        if (currentMoveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleSlideMovement()
    {
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

        steeringDirection = Vector3.ProjectOnPlane(steeringDirection, currentGroundNormal);

        if (steeringDirection.sqrMagnitude > 0.01f)
        {
            steeringDirection.Normalize();
        }

        Vector3 downhillDirection = Vector3.ProjectOnPlane(Vector3.down, currentGroundNormal);

        if (downhillDirection.sqrMagnitude > 0.01f)
        {
            downhillDirection.Normalize();
        }

        Vector3 targetSlideVelocity = downhillDirection * maxSlideSpeed + steeringDirection * slideSteeringStrength;
        targetSlideVelocity = Vector3.ClampMagnitude(targetSlideVelocity, maxSlideSpeed);
        slideVelocity = Vector3.MoveTowards(slideVelocity, targetSlideVelocity, slideAcceleration * Time.deltaTime);
        currentMoveDirection = slideVelocity.sqrMagnitude > 0.01f ? slideVelocity.normalized : downhillDirection;

        characterController.Move((slideVelocity + Vector3.down * slideStickToGroundForce) * Time.deltaTime);

        if (currentMoveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(currentMoveDirection.x, 0f, currentMoveDirection.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        bool recentlyGrounded = Time.time - lastGroundedTime <= crawlGroundedGraceTime;

        if (jumpPressed && isSliding && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isSliding = false;
            slideVelocity = Vector3.zero;
            canDoubleJump = true;
            canUseAirDashOrSpin = true;

            if (resetSpecialActionCooldownOnJump)
            {
                ResetSharedSpecialActionCooldown();
            }
        }
        else if (jumpPressed && recentlyGrounded && isCrawling && crawlTimer >= crawlHoldTimeForHighJump)
        {
            velocity.y = Mathf.Sqrt(crawlHighJumpHeight * -2f * gravity);
            LockAirActionsUntilGrounded();
            isCrawling = false;
            crawlTimer = 0f;
        }
        else if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            canDoubleJump = true;
            canUseAirDashOrSpin = true;

            if (resetSpecialActionCooldownOnJump)
            {
                ResetSharedSpecialActionCooldown();
            }
        }
        else if (jumpPressed && allowDoubleJump && !isGrounded && canDoubleJump && !airActionsLockedUntilGrounded)
        {
            velocity.y = Mathf.Sqrt(doubleJumpHeight * -2f * gravity);
            canDoubleJump = false;
            canUseAirDashOrSpin = true;

            if (resetSpecialActionCooldownOnJump)
            {
                ResetSharedSpecialActionCooldown();
            }
        }

        jumpPressed = false;
    }

    private void HandleGravity()
    {
        if (isUppercutMovementLocked)
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }

        if (isSpinning && !isGrounded && velocity.y < 0f)
        {
            velocity.y += gravity * airSpinGravityMultiplier * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, -airSpinMaxFallSpeed);
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private IEnumerator Dash()
    {
        UseAirDashOrSpin();

        isDashing = true;
        uppercutRequested = false;
        uppercutBufferTimer = 0f;

        Vector3 dashDirection = currentMoveDirection;

        if (dashDirection.sqrMagnitude < 0.01f)
        {
            dashDirection = transform.forward;
        }

        dashDirection.y = 0f;
        dashDirection.Normalize();

        if (dashAttackBox != null)
        {
            dashAttackBox.SetActive(true);
        }

        float timer = 0f;

        while (timer < dashDuration)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        uppercutBufferTimer = uppercutInputBufferTime;

        if (dashAttackBox != null)
        {
            dashAttackBox.SetActive(false);
        }

        float bufferTimer = uppercutInputBufferTime;

        while (bufferTimer > 0f)
        {
            if (uppercutRequested && isGrounded)
            {
                uppercutRequested = false;
                uppercutBufferTimer = 0f;
                StartCoroutine(Uppercut());
                yield break;
            }

            bufferTimer -= Time.deltaTime;
            yield return null;
        }

        uppercutRequested = false;
        uppercutBufferTimer = 0f;

        yield return new WaitForSeconds(dashAttackExtraTime);

        StartSharedSpecialActionCooldown();
    }

    private IEnumerator Spin()
    {
        UseAirDashOrSpin();

        isSpinning = true;

        if (spinAttackBox != null)
        {
            spinAttackBox.SetActive(true);
        }

        yield return new WaitForSeconds(spinDuration);

        if (spinAttackBox != null)
        {
            spinAttackBox.SetActive(false);
        }

        isSpinning = false;

        StartSharedSpecialActionCooldown();
    }

    private IEnumerator GroundSlam()
    {
        UseGroundSlam();

        isGroundSlamming = true;
        velocity = Vector3.zero;

        if (groundSlamAttackBox != null)
        {
            groundSlamAttackBox.SetActive(true);
        }

        float hoverTimer = 0f;

        while (hoverTimer < groundSlamHoverTime)
        {
            hoverTimer += Time.deltaTime;
            yield return null;
        }

        while (!isGrounded)
        {
            velocity.y += groundSlamGravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
            CheckGrounded();
            yield return null;
        }

        velocity.y = -2f;

        yield return new WaitForSeconds(groundSlamImpactTime);

        if (groundSlamAttackBox != null)
        {
            groundSlamAttackBox.SetActive(false);
        }

        isGroundSlamming = false;
    }

    private IEnumerator Uppercut()
    {
        LockAirActionsUntilGrounded();

        isUppercutHovering = true;
        isUppercutMovementLocked = true;
        velocity = Vector3.zero;
        currentMoveDirection = Vector3.zero;

        if (uppercutAttackBox != null)
        {
            uppercutAttackBox.SetActive(true);
        }

        float hoverTimer = 0f;

        while (hoverTimer < uppercutHoverTime)
        {
            characterController.Move(Vector3.zero);
            hoverTimer += Time.deltaTime;
            yield return null;
        }

        velocity = Vector3.zero;
        velocity.y = Mathf.Sqrt(uppercutJumpHeight * -2f * gravity);
        isUppercutHovering = false;

        yield return new WaitForSeconds(uppercutAttackTime);

        if (uppercutAttackBox != null)
        {
            uppercutAttackBox.SetActive(false);
        }

        StartSharedSpecialActionCooldown();
    }
}
