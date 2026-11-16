
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemRequirementInteractable :
    MonoBehaviour,
    IInteractable
{
    public enum RequirementMode
    {
        InventoryItem,
        GlobalCollectible3
    }

    [Header("Requirement")]
    [SerializeField] private RequirementMode requirementMode =
        RequirementMode.InventoryItem;

    [SerializeField] private ItemData requiredItem;
    [SerializeField, Min(1)] private int requiredAmount = 1;
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
        !completed || !onlyUseOnce;

    public void Interact()
    {
        if (!CanInteract)
        {
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

        if (requirementMode == RequirementMode.GlobalCollectible3)
        {
            CheckGlobalCollectibles();
        }
        else
        {
            CheckInventoryItems();
        }
    }

    private void CheckGlobalCollectibles()
    {
        CollectibleManager manager = CollectibleManager.Instance;

        if (manager == null)
        {
            Debug.LogWarning(
                "No CollectibleManager was found."
            );

            return;
        }

        int collectedAmount = GetGlobalCollectible3Total(manager);

        if (collectedAmount < requiredAmount)
        {
            Debug.Log(
                "Collectible requirement failed. Need " +
                requiredAmount +
                " Collectible3. Player has " +
                collectedAmount +
                "."
            );

            onRequirementFailed.Invoke();
            return;
        }

        completed = true;

        Debug.Log(
            "Global Collectible3 requirement completed: " +
            collectedAmount +
            "/" +
            requiredAmount
        );

        onRequirementMet.Invoke();
    }

    private int GetGlobalCollectible3Total(
        CollectibleManager manager
    )
    {
        int totalCollected = 0;

        HashSet<string> countedZones =
            new HashSet<string>();

        foreach (ProgressionDiscDefinition disc in manager.Discs)
        {
            if (disc == null)
            {
                continue;
            }

            foreach (ZoneProgressDefinition zone in disc.Zones)
            {
                if (zone == null ||
                    string.IsNullOrWhiteSpace(zone.ZoneId))
                {
                    continue;
                }

                if (!countedZones.Add(zone.ZoneId))
                {
                    continue;
                }

                totalCollected += manager.GetCollected(
                    zone.ZoneId,
                    CollectibleType.Collectible3
                );
            }
        }

        return totalCollected;
    }

    private void CheckInventoryItems()
    {
        if (requiredItem == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no required ItemData assigned."
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

        if (!inventory.HasItem(requiredItem, requiredAmount))
        {
            Debug.Log(
                "Requirement failed. Need " +
                requiredAmount +
                " " +
                requiredItem.DisplayName +
                ". Player has " +
                inventory.GetItemAmount(requiredItem) +
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
