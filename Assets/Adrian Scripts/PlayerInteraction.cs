using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactionRadius = 2.5f;
    [SerializeField] private LayerMask interactionLayer;

    [Header("UI")]
    [SerializeField] private GameObject interactionPromptUI;
    [SerializeField] private TMP_Text interactionPromptText;

    private IInteractable currentInteractable;
    private bool interactionEnabled = true;

    private void Start()
    {
        HidePrompt();
    }

    private void Update()
    {
        if (!interactionEnabled)
        {
            ClearCurrentInteractable();
            return;
        }

        FindClosestInteractable();
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        if (
            DialogueManager.Instance != null &&
            DialogueManager.Instance.DialogueActive
        )
        {
            DialogueManager.Instance
                .HandleInteract();

            return;
        }

        if (!interactionEnabled)
        {
            return;
        }

        if (currentInteractable == null)
        {
            return;
        }

        if (!currentInteractable.CanInteract)
        {
            return;
        }

        currentInteractable.Interact();
    }

    public void SetInteractionEnabled(bool enabled)
    {
        interactionEnabled = enabled;

        if (!enabled)
        {
            ClearCurrentInteractable();
        }
    }

    private void FindClosestInteractable()
    {
        Collider[] colliders =
            Physics.OverlapSphere(
                transform.position,
                interactionRadius,
                interactionLayer,
                QueryTriggerInteraction.Collide
            );

        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < colliders.Length; i++)
        {
            IInteractable interactable =
                FindInteractable(
                    colliders[i]
                );

            if (interactable == null)
            {
                continue;
            }

            if (!interactable.CanInteract)
            {
                continue;
            }

            float distance =
                Vector3.SqrMagnitude(
                    colliders[i]
                        .ClosestPoint(
                            transform.position
                        ) -
                    transform.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;

                closestInteractable =
                    interactable;
            }
        }

        SetCurrentInteractable(
            closestInteractable
        );
    }

    private IInteractable FindInteractable(
        Collider targetCollider
    )
    {
        MonoBehaviour[] behaviours =
            targetCollider
                .GetComponentsInParent<MonoBehaviour>();

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (
                behaviours[i]
                is IInteractable interactable
            )
            {
                return interactable;
            }
        }

        return null;
    }

    private void SetCurrentInteractable(
        IInteractable interactable
    )
    {
        if (currentInteractable == interactable)
        {
            UpdatePrompt();
            return;
        }

        currentInteractable =
            interactable;

        if (currentInteractable != null)
        {
            ShowPrompt();
        }
        else
        {
            HidePrompt();
        }
    }

    private void ClearCurrentInteractable()
    {
        currentInteractable = null;

        HidePrompt();
    }

    private void ShowPrompt()
    {
        UpdatePrompt();

        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(
                true
            );
        }
    }

    private void UpdatePrompt()
    {
        if (interactionPromptText == null)
        {
            return;
        }

        if (currentInteractable == null)
        {
            interactionPromptText.text =
                string.Empty;

            return;
        }

        interactionPromptText.text =
            currentInteractable
                .InteractionPrompt;
    }

    private void HidePrompt()
    {
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(
                false
            );
        }

        if (interactionPromptText != null)
        {
            interactionPromptText.text =
                string.Empty;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            interactionRadius
        );
    }
}