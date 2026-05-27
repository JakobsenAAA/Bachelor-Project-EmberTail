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

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private bool sprintOnlyWhileGrounded = true;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 16f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashAttackExtraTime = 0.08f;

    [Header("Spin")]
    [SerializeField] private float spinDuration = 0.35f;
    [SerializeField] private float airSpinGravityMultiplier = 0.25f;
    [SerializeField] private float airSpinMaxFallSpeed = 3f;

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
    private Coroutine sharedCooldownCoroutine;
    private bool jumpPressed;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isDashing;
    private bool isSpinning;
    private bool canUseAirSpecialAction = true;
    private bool canUseSharedSpecialAction = true;

    public Vector2 LookInput => lookInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (dashAttackBox != null)
        {
            dashAttackBox.SetActive(false);
        }

        if (spinAttackBox != null)
        {
            spinAttackBox.SetActive(false);
        }
    }

    private void Update()
    {
        CheckGrounded();
        ResetSpecialActionOnLanding();

        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
        }

        HandleGravity();

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
            jumpPressed = true;
        }
    }

    public void OnSprint(InputValue value)
    {
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed && CanStartSpecialAction())
        {
            StartCoroutine(Dash());
        }
    }

    public void OnSpin(InputValue value)
    {
        if (value.isPressed && CanStartSpecialAction())
        {
            StartCoroutine(Spin());
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
    }

    private void ResetSpecialActionOnLanding()
    {
        if (!wasGrounded && isGrounded)
        {
            canUseAirSpecialAction = true;
        }

        if (isGrounded && !isDashing && !isSpinning)
        {
            canUseAirSpecialAction = true;
        }
    }

    private bool CanStartSpecialAction()
    {
        if (!canUseSharedSpecialAction)
        {
            return false;
        }

        if (isDashing || isSpinning)
        {
            return false;
        }

        if (limitAirSpecialActions && !isGrounded && !canUseAirSpecialAction)
        {
            return false;
        }

        return true;
    }

    private void UseSpecialAction()
    {
        canUseSharedSpecialAction = false;

        if (sharedCooldownCoroutine != null)
        {
            StopCoroutine(sharedCooldownCoroutine);
        }

        if (limitAirSpecialActions && !isGrounded)
        {
            canUseAirSpecialAction = false;
        }
    }

    private void ResetSharedSpecialActionCooldown()
    {
        if (sharedCooldownCoroutine != null)
        {
            StopCoroutine(sharedCooldownCoroutine);
            sharedCooldownCoroutine = null;
        }

        canUseSharedSpecialAction = true;
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
        canUseSharedSpecialAction = true;
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
        bool canSprint = sprintHeld && (!sprintOnlyWhileGrounded || isGrounded);
        float currentSpeed = canSprint ? sprintSpeed : moveSpeed;

        characterController.Move(currentMoveDirection * currentSpeed * Time.deltaTime);

        if (currentMoveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentMoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            canUseAirSpecialAction = true;

            if (resetSpecialActionCooldownOnJump)
            {
                ResetSharedSpecialActionCooldown();
            }
        }

        jumpPressed = false;
    }

    private void HandleGravity()
    {
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
        UseSpecialAction();

        isDashing = true;

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

        yield return new WaitForSeconds(dashAttackExtraTime);

        if (dashAttackBox != null)
        {
            dashAttackBox.SetActive(false);
        }

        StartSharedSpecialActionCooldown();
    }

    private IEnumerator Spin()
    {
        UseSpecialAction();

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
}