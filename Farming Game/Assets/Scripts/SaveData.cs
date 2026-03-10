using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
[System.Serializable]
public class SaveData 
{
    public Vector3 playerPosition;
    public string mapBoundary; // boundary name for map
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;
    public List<ChestSaveData> chestSaveData;
}

[System.Serializable]

public class ChestSaveData

{
    public string chestID;
    public bool isOpened;
}
