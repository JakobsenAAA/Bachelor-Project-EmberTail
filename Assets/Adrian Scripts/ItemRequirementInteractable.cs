using UnityEngine;
using UnityEngine.Events;

public class ItemRequirementInteractable :
    MonoBehaviour,
    IInteractable
{
    [Header("Requirement")]
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private int requiredAmount = 1;
    [SerializeField] private bool consumeItems = true;

    [Header("Interaction")]
    [SerializeField] private string interactionPrompt =
        "Interact";

    [SerializeField] private bool onlyUseOnce = true;

    [Header("Events")]
    [SerializeField] private UnityEvent onRequirementMet;
    [SerializeField] private UnityEvent onRequirementFailed;

    private bool completed;

    public string InteractionPrompt =>
        interactionPrompt;

    public bool CanInteract =>
        !completed ||
        !onlyUseOnce;

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        if (requiredItem == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no required ItemData assigned."
            );

            return;
        }

        if (requiredAmount <= 0)
        {
            Debug.LogWarning(
                gameObject.name +
                " has an invalid required amount."
            );

            return;
        }

        PlayerInventory inventory =
            FindFirstObjectByType<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning(
                "No PlayerInventory was found."
            );

            return;
        }

        if (
            !inventory.HasItem(
                requiredItem,
                requiredAmount
            )
        )
        {
            Debug.Log(
                "Requirement failed. Need " +
                requiredAmount +
                " " +
                requiredItem.DisplayName +
                ". Player has " +
                inventory.GetItemAmount(
                    requiredItem
                ) +
                "."
            );

            onRequirementFailed.Invoke();

            return;
        }

        if (consumeItems)
        {
            inventory.RemoveItem(
                requiredItem,
                requiredAmount
            );
        }

        completed = true;

        Debug.Log(
            "Requirement completed: " +
            requiredAmount +
            " " +
            requiredItem.DisplayName
        );

        onRequirementMet.Invoke();
    }
}