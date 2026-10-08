
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

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 0.7f;
    [SerializeField] private bool randomizePitch = true;
    [SerializeField, Range(0.5f, 2f)] private float minimumPitch = 0.9f;
    [SerializeField, Range(0.5f, 2f)] private float maximumPitch = 1.1f;

    private bool collected;

    public string PickupId => pickupId;
    public string ZoneId => zoneId;
    public CollectibleType CollectibleType => collectibleType;
    public int Amount => amount;

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
        pickupId = newPickupId;
        zoneId = newZoneId;
        collectibleType = newCollectibleType;
        amount = Mathf.Max(1, newAmount);

        RefreshCollectedState();
    }

    private void RefreshCollectedState()
    {
        if (
            CollectibleManager.Instance != null &&
            CollectibleManager.Instance.IsPickupCollected(pickupId)
        )
        {
            collected = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
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
            CollectibleManager.Instance.CollectPickup(
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

        PlayPickupSound();

        if (collectEffect != null)
        {
            Instantiate(
                collectEffect,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
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
                "CollectiblePickupAudio"
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

    [ContextMenu("Generate New Pickup ID")]
    private void GenerateNewPickupId()
    {
        pickupId = Guid.NewGuid().ToString();
    }
}
