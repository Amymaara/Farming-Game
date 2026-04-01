using System.Collections.Generic;
using UnityEngine;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).

[System.Serializable]

public class SaveData 
{
    public Vector3 playerPosition;
    public string mapBoundary; // boundary name for map
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;
    public List<ChestSaveData> chestSaveData;
    public List<QuestProgress> questProgressData;
    public List<string> handInQuestIDs;
    public int playerGold;
    public List<ShopInstanceData> shopStates = new();

}

[System.Serializable]

public class ChestSaveData

{
    public string chestID;
    public bool isOpened;
}

[System.Serializable]

public class ShopInstanceData
{
    public string shopID;
    public List<ShopItemData> stock = new();
}

[System.Serializable]

public class ShopItemData
{
    public int itemID;
    public int quantity;
}