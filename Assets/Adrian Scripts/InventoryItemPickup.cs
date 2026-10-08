
using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemPickup : MonoBehaviour, IInteractable
{
    private static readonly HashSet<string> collectedPickupIds =
        new HashSet<string>();

    [Header("Identity")]
    [SerializeField] private string pickupId;

    [Header("Item")]
    [SerializeField] private ItemData item;
    [SerializeField] private int amount = 1;

    [Header("Interaction")]
    [SerializeField] private string interactionPrompt = "Pick Up";

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 0.7f;
    [SerializeField] private bool randomizePitch = true;
    [SerializeField, Range(0.5f, 2f)] private float minimumPitch = 0.9f;
    [SerializeField, Range(0.5f, 2f)] private float maximumPitch = 1.1f;

    private bool collected;

    public string InteractionPrompt => interactionPrompt;

    public bool CanInteract =>
        !collected &&
        item != null &&
        amount > 0;

    private void Start()
    {
        RefreshCollectedState();
    }

    public void Interact()
    {
        if (!CanInteract)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(pickupId))
        {
            Debug.LogWarning(
                gameObject.name + " has no Inventory Pickup ID."
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

        inventory.AddItem(item, amount);

        collectedPickupIds.Add(pickupId);
        collected = true;

        PlayPickupSound();

        gameObject.SetActive(false);
    }

    private void PlayPickupSound()
    {
        if (pickupSound == null)
        {
            return;
        }

        if (randomizePitch)
        {
            float pitch = UnityEngine.Random.Range(
                Mathf.Min(minimumPitch, maximumPitch),
                Mathf.Max(minimumPitch, maximumPitch)
            );

            GameObject audioObject = new GameObject(
                "InventoryPickupAudio"
            );

            AudioSource source =
                audioObject.AddComponent<AudioSource>();

            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.volume = pickupVolume;
            source.pitch = pitch;
            source.clip = pickupSound;
            source.Play();

            Destroy(
                audioObject,
                pickupSound.length / Mathf.Max(0.01f, pitch) + 0.1f
            );
        }
        else if (UIAudioManager.Instance != null)
        {
            UIAudioManager.Instance.PlaySound(
                pickupSound,
                pickupVolume
            );
        }
    }

    public static List<string> CreateCollectedPickupSaveData()
    {
        return new List<string>(collectedPickupIds);
    }

    public static void RestoreCollectedPickupSaveData(
        List<string> savedPickupIds
    )
    {
        collectedPickupIds.Clear();

        if (savedPickupIds != null)
        {
            for (int i = 0; i < savedPickupIds.Count; i++)
            {
                string savedId = savedPickupIds[i];

                if (string.IsNullOrWhiteSpace(savedId))
                {
                    continue;
                }

                collectedPickupIds.Add(savedId);
            }
        }

        InventoryItemPickup[] pickups =
            FindObjectsByType<InventoryItemPickup>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        for (int i = 0; i < pickups.Length; i++)
        {
            pickups[i].RefreshCollectedState();
        }
    }

    public static void ResetCollectedPickups()
    {
        collectedPickupIds.Clear();
    }

    private void RefreshCollectedState()
    {
        if (string.IsNullOrWhiteSpace(pickupId))
        {
            return;
        }

        if (collectedPickupIds.Contains(pickupId))
        {
            collected = true;
            gameObject.SetActive(false);
        }
        else
        {
            collected = false;
        }
    }

    [ContextMenu("Generate New Pickup ID")]
    private void GenerateNewPickupId()
    {
        pickupId = Guid.NewGuid().ToString();
    }
}
