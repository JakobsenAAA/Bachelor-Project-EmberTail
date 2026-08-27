using System;
using UnityEngine;

[Serializable]
public class ZoneProgressDefinition
{
    [Header("Identity")]
    [SerializeField] private string zoneId;
    [SerializeField] private string displayName = "???";

    [TextArea]
    [SerializeField] private string description;

    [Header("Progression")]
    [SerializeField] private bool showInProgression = true;
    [SerializeField] private bool locked;

    [Header("Collectible Totals")]
    [SerializeField] private int collectible1Total;
    [SerializeField] private int collectible2Total;
    [SerializeField] private int collectible3Total;

    public string ZoneId => zoneId;
    public string DisplayName => displayName;
    public string Description => description;
    public bool ShowInProgression => showInProgression;
    public bool Locked => locked;

    public int Collectible1Total => collectible1Total;
    public int Collectible2Total => collectible2Total;
    public int Collectible3Total => collectible3Total;

    public int GetTotal(CollectibleType type)
    {
        switch (type)
        {
            case CollectibleType.Collectible1:
                return collectible1Total;

            case CollectibleType.Collectible2:
                return collectible2Total;

            case CollectibleType.Collectible3:
                return collectible3Total;

            default:
                return 0;
        }
    }
}