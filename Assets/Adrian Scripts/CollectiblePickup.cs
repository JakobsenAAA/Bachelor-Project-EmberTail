using System;
using UnityEngine;

public class CollectiblePickup : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string pickupId;

    [Header("Collectible")]
    [SerializeField] private string zoneId;
    [SerializeField] private CollectibleType collectibleType;
    [SerializeField] private int amount = 1;

    [Header("Feedback")]
    [SerializeField] private GameObject collectEffect;

    private bool collected;

    public string PickupId =>
        pickupId;

    public string ZoneId =>
        zoneId;

    public CollectibleType CollectibleType =>
        collectibleType;

    public int Amount =>
        amount;

    private void Start()
    {
        RefreshCollectedState();
    }

    public void Configure(
        string newPickupId,
        string newZoneId,
        CollectibleType newCollectibleType,
        int newAmount
    )
    {
        pickupId =
            newPickupId;

        zoneId =
            newZoneId;

        collectibleType =
            newCollectibleType;

        amount =
            Mathf.Max(
                1,
                newAmount
            );

        RefreshCollectedState();
    }

    private void RefreshCollectedState()
    {
        if (
            CollectibleManager.Instance != null &&
            CollectibleManager.Instance
                .IsPickupCollected(
                    pickupId
                )
        )
        {
            collected = true;

            gameObject.SetActive(
                false
            );
        }
    }

    private void OnTriggerEnter(
        Collider other
    )
    {
        if (collected)
        {
            return;
        }

        PlayerHealth playerHealth =
            other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth =
                other.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            return;
        }

        Collect();
    }

    private void Collect()
    {
        if (CollectibleManager.Instance == null)
        {
            return;
        }

        bool successfullyCollected =
            CollectibleManager.Instance
                .CollectPickup(
                    pickupId,
                    zoneId,
                    collectibleType,
                    amount
                );

        if (!successfullyCollected)
        {
            return;
        }

        collected = true;

        if (collectEffect != null)
        {
            Instantiate(
                collectEffect,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(
            gameObject
        );
    }

    [ContextMenu("Generate New Pickup ID")]
    private void GenerateNewPickupId()
    {
        pickupId =
            Guid.NewGuid().ToString();
    }
}