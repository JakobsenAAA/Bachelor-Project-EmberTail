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
}

[Serializable]
public class ZoneCollectibleSaveData
{
    public string zoneId;
    public int collectible1;
    public int collectible2;
    public int collectible3;
}