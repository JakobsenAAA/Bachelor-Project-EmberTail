
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EnvironmentalDamageBox : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField, Range(1, 3)] private int damage = 1;
    [SerializeField] private bool instantDeath;
    [SerializeField] private bool continuousDamage = true;
    [SerializeField, Min(0.05f)] private float damageInterval = 1f;

    [Header("Knockback")]
    [SerializeField] private bool enableKnockback = true;
    [SerializeField] private Transform knockbackSource;

    private readonly Dictionary<PlayerHealth, float> nextDamageTimes =
        new Dictionary<PlayerHealth, float>();

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (continuousDamage)
        {
            TryDamage(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            nextDamageTimes.Remove(player);
        }
    }

    private void TryDamage(Collider other)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();

        if (player == null)
        {
            return;
        }

        if (nextDamageTimes.TryGetValue(player, out float nextTime))
        {
            if (!continuousDamage || Time.time < nextTime)
            {
                return;
            }
        }

        nextDamageTimes[player] = Time.time + damageInterval;

        Vector3 sourcePosition = knockbackSource != null
            ? knockbackSource.position
            : GetComponent<Collider>().ClosestPoint(player.transform.position);

        if ((sourcePosition - player.transform.position).sqrMagnitude < 0.01f)
        {
            sourcePosition = transform.position;
        }

        player.TakeDamage(
            damage,
            sourcePosition,
            enableKnockback,
            instantDeath
        );
    }
}
