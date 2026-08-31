using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    [Header("Progression Discs")]
    [SerializeField] private List<ProgressionDiscDefinition> discs =
        new List<ProgressionDiscDefinition>();

    public UnityEvent OnCollectiblesChanged;

    private readonly Dictionary<string, ZoneRuntimeProgress> zoneProgress =
        new Dictionary<string, ZoneRuntimeProgress>();

    private readonly HashSet<string> collectedPickupIds =
        new HashSet<string>();

    public IReadOnlyList<ProgressionDiscDefinition> Discs => discs;

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

        for (int discIndex = 0; discIndex < discs.Count; discIndex++)
        {
            ProgressionDiscDefinition disc = discs[discIndex];

            if (disc == null)
            {
                continue;
            }

            for (int zoneIndex = 0; zoneIndex < disc.Zones.Count; zoneIndex++)
            {
                ZoneProgressDefinition zone = disc.Zones[zoneIndex];

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

                zoneProgress.Add(
                    zone.ZoneId,
                    new ZoneRuntimeProgress()
                );
            }
        }
    }

    public bool CollectPickup(
        string pickupId,
        string zoneId,
        CollectibleType type,
        int amount
    )
    {
        if (string.IsNullOrWhiteSpace(pickupId))
        {
            Debug.LogWarning("CollectiblePickup has no unique Pickup ID.");
            return false;
        }

        if (collectedPickupIds.Contains(pickupId))
        {
            return false;
        }

        if (amount <= 0)
        {
            return false;
        }

        if (!zoneProgress.ContainsKey(zoneId))
        {
            Debug.LogWarning("Unknown Zone ID: " + zoneId);
            return false;
        }

        collectedPickupIds.Add(pickupId);

        AddCollectible(
            zoneId,
            type,
            amount
        );

        return true;
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

        if (!zoneProgress.TryGetValue(
                zoneId,
                out ZoneRuntimeProgress progress
            ))
        {
            Debug.LogWarning("Unknown Zone ID: " + zoneId);
            return;
        }

        int total = GetTotal(
            zoneId,
            type
        );

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

    public bool IsPickupCollected(string pickupId)
    {
        if (string.IsNullOrWhiteSpace(pickupId))
        {
            return false;
        }

        return collectedPickupIds.Contains(pickupId);
    }

    public int GetCollected(
        string zoneId,
        CollectibleType type
    )
    {
        if (!zoneProgress.TryGetValue(
                zoneId,
                out ZoneRuntimeProgress progress
            ))
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
        ZoneProgressDefinition zone =
            GetZone(zoneId);

        if (zone == null)
        {
            return 0;
        }

        return zone.GetTotal(type);
    }

    public ZoneProgressDefinition GetZone(
        string zoneId
    )
    {
        for (int discIndex = 0; discIndex < discs.Count; discIndex++)
        {
            ProgressionDiscDefinition disc =
                discs[discIndex];

            if (disc == null)
            {
                continue;
            }

            for (int zoneIndex = 0; zoneIndex < disc.Zones.Count; zoneIndex++)
            {
                ZoneProgressDefinition zone =
                    disc.Zones[zoneIndex];

                if (
                    zone != null &&
                    zone.ZoneId == zoneId
                )
                {
                    return zone;
                }
            }
        }

        return null;
    }

    public ProgressionDiscDefinition GetDisc(
        string discId
    )
    {
        for (int i = 0; i < discs.Count; i++)
        {
            if (
                discs[i] != null &&
                discs[i].DiscId == discId
            )
            {
                return discs[i];
            }
        }

        return null;
    }

    public ProgressionDiscDefinition GetDiscContainingZone(
        string zoneId
    )
    {
        for (int discIndex = 0; discIndex < discs.Count; discIndex++)
        {
            ProgressionDiscDefinition disc =
                discs[discIndex];

            if (disc == null)
            {
                continue;
            }

            for (int zoneIndex = 0; zoneIndex < disc.Zones.Count; zoneIndex++)
            {
                ZoneProgressDefinition zone =
                    disc.Zones[zoneIndex];

                if (
                    zone != null &&
                    zone.ZoneId == zoneId
                )
                {
                    return disc;
                }
            }
        }

        return null;
    }

    public int GetDiscIndexContainingZone(
        string zoneId
    )
    {
        for (int discIndex = 0; discIndex < discs.Count; discIndex++)
        {
            ProgressionDiscDefinition disc =
                discs[discIndex];

            if (disc == null)
            {
                continue;
            }

            for (int zoneIndex = 0; zoneIndex < disc.Zones.Count; zoneIndex++)
            {
                ZoneProgressDefinition zone =
                    disc.Zones[zoneIndex];

                if (
                    zone != null &&
                    zone.ZoneId == zoneId
                )
                {
                    return discIndex;
                }
            }
        }

        return -1;
    }

    public int GetZoneIndex(
        string zoneId
    )
    {
        ProgressionDiscDefinition disc =
            GetDiscContainingZone(zoneId);

        if (disc == null)
        {
            return -1;
        }

        for (int i = 0; i < disc.Zones.Count; i++)
        {
            ZoneProgressDefinition zone =
                disc.Zones[i];

            if (
                zone != null &&
                zone.ZoneId == zoneId
            )
            {
                return i;
            }
        }

        return -1;
    }

    public int GetProgressionDiscCount()
    {
        int count = 0;

        for (int i = 0; i < discs.Count; i++)
        {
            if (
                discs[i] != null &&
                discs[i].ShowInProgression
            )
            {
                count++;
            }
        }

        return count;
    }

    public ProgressionDiscDefinition GetProgressionDisc(
        int progressionIndex
    )
    {
        int currentIndex = 0;

        for (int i = 0; i < discs.Count; i++)
        {
            ProgressionDiscDefinition disc =
                discs[i];

            if (
                disc == null ||
                !disc.ShowInProgression
            )
            {
                continue;
            }

            if (currentIndex == progressionIndex)
            {
                return disc;
            }

            currentIndex++;
        }

        return null;
    }

    public List<ZoneCollectibleSaveData> CreateZoneSaveData()
    {
        List<ZoneCollectibleSaveData> saveData =
            new List<ZoneCollectibleSaveData>();

        foreach (
            KeyValuePair<string, ZoneRuntimeProgress> entry
            in zoneProgress
        )
        {
            ZoneCollectibleSaveData zoneData =
                new ZoneCollectibleSaveData();

            zoneData.zoneId = entry.Key;
            zoneData.collectible1 = entry.Value.collectible1;
            zoneData.collectible2 = entry.Value.collectible2;
            zoneData.collectible3 = entry.Value.collectible3;

            saveData.Add(zoneData);
        }

        return saveData;
    }

    public List<string> CreateCollectedPickupSaveData()
    {
        return new List<string>(
            collectedPickupIds
        );
    }

    public void RestoreSaveData(
        List<ZoneCollectibleSaveData> savedZones,
        List<string> savedPickupIds
    )
    {
        ResetProgressInternal();

        if (savedZones != null)
        {
            for (int i = 0; i < savedZones.Count; i++)
            {
                ZoneCollectibleSaveData savedZone =
                    savedZones[i];

                if (
                    savedZone == null ||
                    !zoneProgress.TryGetValue(
                        savedZone.zoneId,
                        out ZoneRuntimeProgress progress
                    )
                )
                {
                    continue;
                }

                progress.collectible1 =
                    Mathf.Clamp(
                        savedZone.collectible1,
                        0,
                        GetTotal(
                            savedZone.zoneId,
                            CollectibleType.Collectible1
                        )
                    );

                progress.collectible2 =
                    Mathf.Clamp(
                        savedZone.collectible2,
                        0,
                        GetTotal(
                            savedZone.zoneId,
                            CollectibleType.Collectible2
                        )
                    );

                progress.collectible3 =
                    Mathf.Clamp(
                        savedZone.collectible3,
                        0,
                        GetTotal(
                            savedZone.zoneId,
                            CollectibleType.Collectible3
                        )
                    );
            }
        }

        collectedPickupIds.Clear();

        if (savedPickupIds != null)
        {
            for (int i = 0; i < savedPickupIds.Count; i++)
            {
                string pickupId =
                    savedPickupIds[i];

                if (!string.IsNullOrWhiteSpace(pickupId))
                {
                    collectedPickupIds.Add(pickupId);
                }
            }
        }

        OnCollectiblesChanged.Invoke();
    }

    public void ResetProgress()
    {
        ResetProgressInternal();
        collectedPickupIds.Clear();

        OnCollectiblesChanged.Invoke();
    }

    private void ResetProgressInternal()
    {
        foreach (
            KeyValuePair<string, ZoneRuntimeProgress> entry
            in zoneProgress
        )
        {
            entry.Value.collectible1 = 0;
            entry.Value.collectible2 = 0;
            entry.Value.collectible3 = 0;
        }
    }

    private class ZoneRuntimeProgress
    {
        public int collectible1;
        public int collectible2;
        public int collectible3;
    }
}