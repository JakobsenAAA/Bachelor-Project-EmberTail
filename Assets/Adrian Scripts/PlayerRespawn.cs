using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerRespawn : MonoBehaviour
{
    [Header("Starting Respawn")]
    [SerializeField] private Transform startingRespawnPoint;
    [SerializeField] private string startingCheckpointId = "start";
    [SerializeField] private string startingZoneId = "zone1";

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private EnemyRespawnManager enemyRespawnManager;

    [Header("Death Transition")]
    [SerializeField] private bool useDeathTransition = true;
    [SerializeField] private float deathFadeOutDuration = 0.75f;
    [SerializeField] private float deathLoadingDuration = 1.5f;
    [SerializeField] private float respawnFadeInDuration = 0.75f;

    private CharacterController characterController;
    private Vector3 currentRespawnPosition;
    private Quaternion currentRespawnRotation;
    private string currentCheckpointId;
    private string currentZoneId;
    private bool deathTransitionActive;

    public string CurrentCheckpointId =>
        currentCheckpointId;

    public string CurrentZoneId =>
        currentZoneId;

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController>();

        if (playerHealth == null)
        {
            playerHealth =
                GetComponent<PlayerHealth>();
        }

        if (enemyRespawnManager == null)
        {
            enemyRespawnManager =
                FindFirstObjectByType<
                    EnemyRespawnManager
                >();
        }

        if (startingRespawnPoint != null)
        {
            currentRespawnPosition =
                startingRespawnPoint.position;

            currentRespawnRotation =
                startingRespawnPoint.rotation;

            RespawnPoint startingPoint =
                startingRespawnPoint
                    .GetComponent<RespawnPoint>();

            if (startingPoint != null)
            {
                currentCheckpointId =
                    startingPoint.CheckpointId;

                currentZoneId =
                    startingPoint.ZoneId;
            }
            else
            {
                currentCheckpointId =
                    startingCheckpointId;

                currentZoneId =
                    startingZoneId;
            }
        }
        else
        {
            currentRespawnPosition =
                transform.position;

            currentRespawnRotation =
                transform.rotation;

            currentCheckpointId =
                startingCheckpointId;

            currentZoneId =
                startingZoneId;
        }
    }

    private void Start()
    {
        UpdateZoneManager();
    }

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth
                .OnPlayerDied
                .AddListener(
                    HandlePlayerDied
                );
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth
                .OnPlayerDied
                .RemoveListener(
                    HandlePlayerDied
                );
        }
    }

    public void SetRespawnPoint(
        RespawnPoint respawnPoint
    )
    {
        if (respawnPoint == null)
        {
            return;
        }

        currentRespawnPosition =
            respawnPoint.transform.position;

        currentRespawnRotation =
            respawnPoint.transform.rotation;

        currentCheckpointId =
            respawnPoint.CheckpointId;

        currentZoneId =
            respawnPoint.ZoneId;
    }

    public bool LoadCheckpoint(
        string checkpointId
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                checkpointId
            )
        )
        {
            return false;
        }

        RespawnPoint[] respawnPoints =
            FindObjectsByType<RespawnPoint>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        for (
            int i = 0;
            i < respawnPoints.Length;
            i++
        )
        {
            RespawnPoint respawnPoint =
                respawnPoints[i];

            if (
                respawnPoint != null &&
                respawnPoint.CheckpointId ==
                checkpointId
            )
            {
                SetRespawnPoint(
                    respawnPoint
                );

                Respawn();

                return true;
            }
        }

        Debug.LogWarning(
            "Could not find saved checkpoint: " +
            checkpointId
        );

        return false;
    }

    private void HandlePlayerDied()
    {
        if (deathTransitionActive)
        {
            return;
        }

        if (
            useDeathTransition &&
            LoadingScreenManager.Instance != null
        )
        {
            deathTransitionActive = true;

            LoadingScreenManager.Instance
                .StartRespawnTransition(
                    CompleteDeathRespawn,
                    deathFadeOutDuration,
                    deathLoadingDuration,
                    respawnFadeInDuration
                );

            return;
        }

        Respawn();
    }

    private void CompleteDeathRespawn()
    {
        Respawn();

        deathTransitionActive =
            false;
    }

    public void Respawn()
    {
        characterController.enabled =
            false;

        transform.SetPositionAndRotation(
            currentRespawnPosition,
            currentRespawnRotation
        );

        characterController.enabled =
            true;

        UpdateZoneManager();

        if (playerHealth != null)
        {
            playerHealth
                .RestoreFullHealth();
        }

        if (enemyRespawnManager != null)
        {
            enemyRespawnManager
                .ResetAllEnemies();
        }
    }

    private void UpdateZoneManager()
    {
        if (ZoneManager.Instance == null)
        {
            return;
        }

        ZoneManager.Instance
            .ForceCurrentZone(
                currentZoneId
            );
    }
}