
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CrusherDamageBox : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CeilingCrusher ceilingCrusher;

    [Header("Damage")]
    [SerializeField, Range(1, 3)] private int damage = 1;
    [SerializeField] private bool instantDeath;

    [Header("Knockback")]
    [SerializeField] private bool enableKnockback;
    [SerializeField] private Transform knockbackSource;

    [Header("Squash")]
    [SerializeField] private bool enableSquash = true;

    private bool hasHitThisDrop;
    private bool damageActive;

    private void Awake()
    {
        if (ceilingCrusher == null)
        {
            ceilingCrusher = GetComponentInParent<CeilingCrusher>();
        }
    }

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryCrush(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryCrush(other);
    }

    private void TryCrush(Collider other)
    {
        if (!damageActive || hasHitThisDrop)
        {
            return;
        }

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null || playerHealth.CurrentHitPoints <= 0)
        {
            return;
        }

        hasHitThisDrop = true;

        Vector3 sourcePosition = knockbackSource != null
            ? knockbackSource.position
            : transform.position;

        playerHealth.TakeDamage(
            damage,
            sourcePosition,
            enableKnockback,
            instantDeath
        );

        if (enableSquash && playerHealth.CurrentHitPoints > 0)
        {
            PlayerSquashEffect squashEffect =
                playerHealth.GetComponent<PlayerSquashEffect>();

            if (squashEffect != null)
            {
                squashEffect.ApplySquash();
            }
        }
    }

    public void ResetHit()
    {
        hasHitThisDrop = false;
    }

    public void SetDamageActive(bool active)
    {
        damageActive = active;
    }
}
