using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressionDiscDefinition
{
    [Header("Identity")]
    [SerializeField] private string discId;
    [SerializeField] private string displayName = "???";

    [Header("Progression")]
    [SerializeField] private bool showInProgression = true;
    [SerializeField] private bool locked;

    [Header("Zones")]
    [SerializeField] private List<ZoneProgressDefinition> zones =
        new List<ZoneProgressDefinition>();

    public string DiscId => discId;
    public string DisplayName => displayName;
    public bool ShowInProgression => showInProgression;
    public bool Locked => locked;
    public IReadOnlyList<ZoneProgressDefinition> Zones => zones;

    public int ZoneCount => zones.Count;

    public ZoneProgressDefinition GetZone(int index)
    {
        if (index < 0 || index >= zones.Count)
        {
            return null;
        }

        return zones[index];
    }
}