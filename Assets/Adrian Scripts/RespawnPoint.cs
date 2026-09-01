using System;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string checkpointId;
    [SerializeField] private string zoneId;

    [Header("Feedback")]
    [SerializeField] private bool showCheckpointNotification = true;
    [SerializeField] private SaveNotificationUI notificationUI;
    [SerializeField] private string notificationMessage = "CHECKPOINT REACHED";

    public string CheckpointId => checkpointId;
    public string ZoneId => zoneId;

    private void OnTriggerEnter(Collider other)
    {
        PlayerRespawn playerRespawn =
            other.GetComponent<PlayerRespawn>();

        if (playerRespawn == null)
        {
            return;
        }

        bool isNewCheckpoint =
            playerRespawn.CurrentCheckpointId !=
            checkpointId;

        playerRespawn.SetRespawnPoint(this);

        if (
            isNewCheckpoint &&
            showCheckpointNotification
        )
        {
            ShowNotification();
        }
    }

    private void ShowNotification()
    {
        if (notificationUI == null)
        {
            notificationUI =
                FindFirstObjectByType<
                    SaveNotificationUI
                >();
        }

        if (notificationUI == null)
        {
            return;
        }

        notificationUI.ShowMessage(
            notificationMessage
        );
    }

    [ContextMenu("Generate New Checkpoint ID")]
    private void GenerateNewCheckpointId()
    {
        checkpointId =
            Guid.NewGuid().ToString();
    }
}