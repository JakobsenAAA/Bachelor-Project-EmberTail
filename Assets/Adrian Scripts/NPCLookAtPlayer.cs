using UnityEngine;

public class NPCLookAtPlayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Turning")]
    [SerializeField] private bool turnTowardsPlayer = true;
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private bool onlyTurnWhenNearby = true;
    [SerializeField] private float turnDistance = 5f;

    [Header("Rotation")]
    [SerializeField] private bool lockXRotation = true;
    [SerializeField] private bool lockZRotation = true;

    private void Start()
    {
        if (player == null)
        {
            PlayerController playerController =
                FindFirstObjectByType<PlayerController>();

            if (playerController != null)
            {
                player =
                    playerController.transform;
            }
        }
    }

    private void Update()
    {
        if (!turnTowardsPlayer)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        Vector3 direction =
            player.position -
            transform.position;

        if (onlyTurnWhenNearby)
        {
            float distance =
                direction.magnitude;

            if (distance > turnDistance)
            {
                return;
            }
        }

        if (lockXRotation)
        {
            direction.y = 0f;
        }

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction,
                Vector3.up
            );

        Vector3 targetEuler =
            targetRotation.eulerAngles;

        Vector3 currentEuler =
            transform.rotation.eulerAngles;

        if (lockXRotation)
        {
            targetEuler.x =
                currentEuler.x;
        }

        if (lockZRotation)
        {
            targetEuler.z =
                currentEuler.z;
        }

        targetRotation =
            Quaternion.Euler(
                targetEuler
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed *
                Time.deltaTime
            );
    }

    private void OnDrawGizmosSelected()
    {
        if (!onlyTurnWhenNearby)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            transform.position,
            turnDistance
        );
    }
}