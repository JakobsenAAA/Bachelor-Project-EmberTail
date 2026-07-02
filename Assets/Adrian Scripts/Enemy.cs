using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0f;
    [SerializeField] private GameObject deathEffect;

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
}