
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FallDeathBox : MonoBehaviour
{
    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryTriggerDeath(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryTriggerDeath(other);
    }

    private void TryTriggerDeath(Collider other)
    {
        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null || playerHealth.CurrentHitPoints <= 0)
        {
            return;
        }

        PlayerRespawn playerRespawn =
            playerHealth.GetComponent<PlayerRespawn>();

        if (playerRespawn == null)
        {
            return;
        }

        playerRespawn.BeginFallDeath();

        playerHealth.TakeDamage(
            playerHealth.CurrentHitPoints,
            transform.position,
            false,
            true
        );
    }
}
