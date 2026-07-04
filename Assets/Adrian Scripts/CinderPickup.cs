using UnityEngine;

public class CinderPickup : MonoBehaviour
{
    [SerializeField] private int cinderValue = 1;
    [SerializeField] private float attractRadius = 4f;
    [SerializeField] private float collectDistance = 0.6f;
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private GameObject collectEffect;

    private static PlayerHealth cachedPlayer;
    private PlayerHealth targetPlayer;
    private float currentSpeed;
    private bool collected;

    private void Start()
    {
        if (cachedPlayer == null)
        {
            cachedPlayer = FindFirstObjectByType<PlayerHealth>();
        }

        targetPlayer = cachedPlayer;
    }

    private void Update()
    {
        if (collected || targetPlayer == null)
        {
            return;
        }

        Vector3 targetPosition = targetPlayer.transform.position + Vector3.up * 0.8f;
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        if (distance > attractRadius)
        {
            currentSpeed = 0f;
            return;
        }

        if (distance <= collectDistance)
        {
            Collect(targetPlayer);
            return;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.deltaTime);
        transform.position += direction.normalized * currentSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryCollect(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryCollect(other);
    }

    private void TryCollect(Collider other)
    {
        if (collected)
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        Collect(playerHealth);
    }

    private void Collect(PlayerHealth playerHealth)
    {
        collected = true;
        playerHealth.AddCinders(cinderValue);

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}