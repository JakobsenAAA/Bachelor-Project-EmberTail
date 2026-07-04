using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Death")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0f;
    [SerializeField] private GameObject deathEffect;

    [Header("Drops")]
    [SerializeField] private GameObject cinderPrefab;
    [SerializeField] private int cinderDropAmount = 5;
    [SerializeField] private float dropSpreadRadius = 0.75f;
    [SerializeField] private float dropHeight = 0.5f;

    public bool IsDead { get; private set; }

    public void Die()
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, transform.rotation);
        }

        DropCinders();

        Collider[] colliders = GetComponentsInChildren<Collider>();

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private void DropCinders()
    {
        if (cinderPrefab == null)
        {
            return;
        }

        for (int i = 0; i < cinderDropAmount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * dropSpreadRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, dropHeight, randomCircle.y);
            Instantiate(cinderPrefab, spawnPosition, Quaternion.identity);
        }
    }
}