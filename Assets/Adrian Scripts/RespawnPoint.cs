using System;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private string checkpointId;
    [SerializeField] private string zoneId;

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

        playerRespawn.SetRespawnPoint(this);
    }

    [ContextMenu("Generate New Checkpoint ID")]
    private void GenerateNewCheckpointId()
    {
        checkpointId =
            Guid.NewGuid().ToString();
    }
}