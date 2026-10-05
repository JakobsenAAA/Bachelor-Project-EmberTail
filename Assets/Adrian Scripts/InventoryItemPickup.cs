using UnityEngine;

public class InventoryItemPickup :
    MonoBehaviour,
    IInteractable
{
    [Header("Item")]
    [SerializeField] private ItemData item;

    [SerializeField] private int amount = 1;

    [Header("Interaction")]
    [SerializeField] private string interactionPrompt =
        "Pick Up";

    private bool collected;

    public string InteractionPrompt =>
        interactionPrompt;

    public bool CanInteract =>
        !collected &&
        item != null &&
        amount > 0;

    public void Interact()
    {
        if (!CanInteract)
        {
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

        inventory.AddItem(
            item,
            amount
        );

        collected = true;

        gameObject.SetActive(
            false
        );
    }
}