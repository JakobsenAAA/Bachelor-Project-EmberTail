using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("Zones")]
    [SerializeField] private List<ZoneProgressDefinition> zones = new List<ZoneProgressDefinition>();

    public UnityEvent OnCollectiblesChanged;

    private readonly Dictionary<string, ZoneRuntimeProgress> zoneProgress =
        new Dictionary<string, ZoneRuntimeProgress>();

    public IReadOnlyList<ZoneProgressDefinition> Zones => zones;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildZoneProgress();
    }

    private void Start()
    {
        OnCollectiblesChanged.Invoke();
    }

    private void BuildZoneProgress()
    {
        zoneProgress.Clear();

        for (int i = 0; i < zones.Count; i++)
        {
            ZoneProgressDefinition zone = zones[i];

            if (zone == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(zone.ZoneId))
            {
                Debug.LogError("A zone in CollectibleManager has no Zone ID.");
                continue;
            }

            if (zoneProgress.ContainsKey(zone.ZoneId))
            {
                Debug.LogError("Duplicate Zone ID found: " + zone.ZoneId);
                continue;
            }

            zoneProgress.Add(zone.ZoneId, new ZoneRuntimeProgress());
        }
    }

    public void AddCollectible(
        string zoneId,
        CollectibleType type,
        int amount
    )
    {
        if (amount <= 0)
        {
            return;
        }

        if (!zoneProgress.TryGetValue(zoneId, out ZoneRuntimeProgress progress))
        {
            Debug.LogWarning("Unknown Zone ID: " + zoneId);
            return;
        }

        int total = GetTotal(zoneId, type);

        switch (type)
        {
            case CollectibleType.Collectible1:
                progress.collectible1 = Mathf.Clamp(
                    progress.collectible1 + amount,
                    0,
                    total
                );
                break;

            case CollectibleType.Collectible2:
                progress.collectible2 = Mathf.Clamp(
                    progress.collectible2 + amount,
                    0,
                    total
                );
                break;

            case CollectibleType.Collectible3:
                progress.collectible3 = Mathf.Clamp(
                    progress.collectible3 + amount,
                    0,
                    total
                );
                break;
        }

        OnCollectiblesChanged.Invoke();
    }

    public int GetCollected(
        string zoneId,
        CollectibleType type
    )
    {
        if (!zoneProgress.TryGetValue(zoneId, out ZoneRuntimeProgress progress))
        {
            return 0;
        }

        switch (type)
        {
            case CollectibleType.Collectible1:
                return progress.collectible1;

            case CollectibleType.Collectible2:
                return progress.collectible2;

            case CollectibleType.Collectible3:
                return progress.collectible3;

            default:
                return 0;
        }
    }

    public int GetTotal(
        string zoneId,
        CollectibleType type
    )
    {
        ZoneProgressDefinition zone = GetZone(zoneId);

        if (zone == null)
        {
            return 0;
        }

        return zone.GetTotal(type);
    }

    public ZoneProgressDefinition GetZone(string zoneId)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i] != null && zones[i].ZoneId == zoneId)
            {
                return zones[i];
            }
        }

        return null;
    }

    public int GetProgressionZoneCount()
    {
        int count = 0;

        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i] != null && zones[i].ShowInProgression)
            {
                count++;
            }
        }

        return count;
    }

    public ZoneProgressDefinition GetProgressionZone(int progressionIndex)
    {
        int currentIndex = 0;

        for (int i = 0; i < zones.Count; i++)
        {
            ZoneProgressDefinition zone = zones[i];

            if (zone == null || !zone.ShowInProgression)
            {
                continue;
            }

            if (currentIndex == progressionIndex)
            {
                return zone;
            }

            currentIndex++;
        }

        return null;
    }

    public void ResetProgress()
    {
        foreach (KeyValuePair<string, ZoneRuntimeProgress> entry in zoneProgress)
        {
            entry.Value.collectible1 = 0;
            entry.Value.collectible2 = 0;
            entry.Value.collectible3 = 0;
        }

        OnCollectiblesChanged.Invoke();
    }

    private class ZoneRuntimeProgress
    {
        public int collectible1;
        public int collectible2;
        public int collectible3;
    }
}