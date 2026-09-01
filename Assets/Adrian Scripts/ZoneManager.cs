using UnityEngine;
using UnityEngine.Events;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance { get; private set; }

    [Header("Starting Location")]
    [SerializeField] private string startingZoneId = "zone1";

    public UnityEvent OnZoneChanged;

    private string currentZoneId;

    public string CurrentZoneId => currentZoneId;

    public string CurrentDiscId
    {
        get
        {
            if (
                CollectibleManager.Instance == null ||
                string.IsNullOrWhiteSpace(currentZoneId)
            )
            {
                return string.Empty;
            }

            ProgressionDiscDefinition disc =
                CollectibleManager.Instance
                    .GetDiscContainingZone(
                        currentZoneId
                    );

            if (disc == null)
            {
                return string.Empty;
            }

            return disc.DiscId;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        currentZoneId =
            startingZoneId;
    }

    public void SetCurrentZone(string zoneId)
    {
        if (string.IsNullOrWhiteSpace(zoneId))
        {
            return;
        }

        if (currentZoneId == zoneId)
        {
            return;
        }

        currentZoneId =
            zoneId;

        OnZoneChanged.Invoke();
    }

    public void ForceCurrentZone(string zoneId)
    {
        if (string.IsNullOrWhiteSpace(zoneId))
        {
            return;
        }

        currentZoneId =
            zoneId;

        OnZoneChanged.Invoke();
    }
}