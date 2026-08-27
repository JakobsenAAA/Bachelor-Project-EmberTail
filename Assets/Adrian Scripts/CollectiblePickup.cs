using UnityEngine;

public class CollectiblePickup : MonoBehaviour
{
    [SerializeField] private CollectibleType collectibleType;
    [SerializeField] private int amount = 1;
    [SerializeField] private GameObject collectEffect;

    private bool collected;

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = other.GetComponentInParent<PlayerHealth>();
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

        collected = true;

        CollectibleManager.Instance.AddCollectible(
            collectibleType,
            amount
        );

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
}