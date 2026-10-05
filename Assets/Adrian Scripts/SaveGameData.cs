using System;
using System.Collections.Generic;

[Serializable]
public class SaveGameData
{
    public string sceneName;
    public string checkpointId;
    public string zoneId;
    public bool betaCompleted;

    public List<ZoneCollectibleSaveData> zoneProgress =
        new List<ZoneCollectibleSaveData>();

    public List<string> collectedPickupIds =
        new List<string>();

    public List<InventoryItemSaveData> inventoryItems =
        new List<InventoryItemSaveData>();

    public List<string> collectedInventoryPickupIds =
        new List<string>();

    public List<QuestSaveData> quests =
        new List<QuestSaveData>();
}

[Serializable]
public class ZoneCollectibleSaveData
{
    public string zoneId;
    public int collectible1;
    public int collectible2;
    public int collectible3;
}

[Serializable]
public class InventoryItemSaveData
{
    public string itemId;
    public int amount;
}

[Serializable]
public class QuestSaveData
{
    public string questId;
    public QuestState state;
    public int currentProgress;
}