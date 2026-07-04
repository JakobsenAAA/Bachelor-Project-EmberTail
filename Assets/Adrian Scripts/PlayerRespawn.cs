using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Transform startingRespawnPoint;
    [SerializeField] private PlayerHealth playerHealth;

    private CharacterController characterController;
    private Vector3 currentRespawnPosition;
    private Quaternion currentRespawnRotation;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }

        if (startingRespawnPoint != null)
        {
            currentRespawnPosition = startingRespawnPoint.position;
            currentRespawnRotation = startingRespawnPoint.rotation;
        }
        else
        {
            currentRespawnPosition = transform.position;
            currentRespawnRotation = transform.rotation;
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied.AddListener(Respawn);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied.RemoveListener(Respawn);
        }
    }

    public void SetRespawnPoint(Transform respawnPoint)
    {
        currentRespawnPosition = respawnPoint.position;
        currentRespawnRotation = respawnPoint.rotation;
    }

    public void Respawn()
    {
        characterController.enabled = false;
        transform.SetPositionAndRotation(currentRespawnPosition, currentRespawnRotation);
        characterController.enabled = true;

        if (playerHealth != null)
        {
            playerHealth.RestoreFullHealth();
        }
    }
}