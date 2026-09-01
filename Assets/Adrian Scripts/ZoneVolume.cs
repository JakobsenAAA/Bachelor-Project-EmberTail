using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneVolume : MonoBehaviour
{
    [Header("Zone")]
    [SerializeField] private string zoneId = "zone1";

    public string ZoneId => zoneId;

    private void Reset()
    {
        Collider zoneCollider =
            GetComponent<Collider>();

        zoneCollider.isTrigger =
            true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerRespawn playerRespawn =
            other.GetComponent<PlayerRespawn>();

        if (playerRespawn == null)
        {
            return;
        }

        if (ZoneManager.Instance == null)
        {
            return;
        }

        ZoneManager.Instance
            .SetCurrentZone(zoneId);
    }
}