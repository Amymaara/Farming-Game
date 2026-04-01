using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).
public class SaveController : MonoBehaviour
{
   

    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private Chest[] chests;
    private ShopNPC[] shops;

    private void Awake()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    private void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        hotbarController = FindObjectOfType<HotbarController>();
        chests = FindObjectsOfType<Chest>();
        shops = FindObjectsOfType<ShopNPC>();
        StartCoroutine(LoadAfterSetup());
        
    }


    private IEnumerator LoadAfterSetup()
    {
        yield return null; // wait 1 frame so inventory + UI are ready
        LoadGame();
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();


        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            mapBoundary = confiner.BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotbarItems(),
            chestSaveData = GetChestsState(),
            questProgressData = QuestController.Instance.activateQuests,
            handInQuestIDs = QuestController.Instance.handinQuestIDs,
            playerGold = CurrencyController.Instance.GetGold(),
            shopStates = GetShopStates()
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveLocation, json);

        Debug.Log("Game saved.");
        Debug.Log("Saved inventory count: " + saveData.inventorySaveData.Count);
        Debug.Log(json);
    }
    private List<ShopInstanceData> GetShopStates()
    {
        List<ShopInstanceData> shopStates = new List<ShopInstanceData>();
        foreach (var shop in shops)
        {
            ShopInstanceData shopData = new ShopInstanceData()
            {
                shopID = shop.shopID,
                stock = new List<ShopItemData>()
            };

            foreach (var stockItem in shop.GetCurrentStock())
            {
                shopData.stock.Add(new ShopItemData()
                {
                    itemID = stockItem.itemID,
                    quantity = stockItem.quantity,
                });
            }

            shopStates.Add(shopData);   
        }
        return shopStates;
    }
    private List<ChestSaveData> GetChestsState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();

        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.ChestID,
                isOpened = chest.IsOpened
            };

            chestStates.Add(chestSaveData);
        }
        return chestStates;
        
    }

    public void LoadGame()
    {
   

        if (!File.Exists(saveLocation))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string json = File.ReadAllText(saveLocation);
        Debug.Log("Raw JSON: " + json);

        SaveData saveData = JsonUtility.FromJson<SaveData>(json);


        // PLAYER
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = saveData.playerPosition;
            }
            else
            {
                player.transform.position = saveData.playerPosition;
            }
        }

        // CAMERA BOUNDARY
        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        if (confiner != null)
        {
            GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);

            if (boundaryObject != null)
            {
                PolygonCollider2D boundary = boundaryObject.GetComponent<PolygonCollider2D>();

                if (boundary != null)
                {
                    confiner.BoundingShape2D = boundary;
                    confiner.InvalidateBoundingShapeCache();

                }
            }
        }

        // INVENTORY
        if (inventoryController != null)
        {
            inventoryController.SetInventoryItems(saveData.inventorySaveData);

        }

        Debug.Log("Load complete.");

        // HOTBAR 
        if (hotbarController != null)
        {
            hotbarController.SetHotbarItems(saveData.hotbarSaveData);

        }

        // CHESTS

        LoadChestStates(saveData.chestSaveData);

        //LOAD QUEST PROGRESS
        QuestController.Instance.LoadQuestProgress(saveData.questProgressData);

        // SAVE COMPLETED QUESTS
        QuestController.Instance.handinQuestIDs = saveData.handInQuestIDs;

        // SHOP STATES
        LoadShopStates(saveData.shopStates);
        CurrencyController.Instance.SetGold(saveData.playerGold);
    }

    private void LoadChestStates(List<ChestSaveData> chestStates)
    {
        foreach (Chest chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);

            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }
    }

    private void LoadShopStates(List<ShopInstanceData> shopStates)
    {
        if (shopStates == null) return;

        foreach (var shop in shops)
        {
            ShopInstanceData shopData = shopStates.FirstOrDefault( s => s.shopID == shop.shopID );

            if (shopData != null)
            {
                List<ShopNPC.ShopStockItem> loadedStock = new List<ShopNPC.ShopStockItem>();

                foreach (var itemData in shopData.stock)
                {
                    loadedStock.Add(new ShopNPC.ShopStockItem
                    {
                        itemID = itemData.itemID,
                        quantity = itemData.quantity,
                    });
                }

                shop.SetStock(loadedStock);
            }
        }
    }

    public void ClearSave()
    {
        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
            Debug.Log("Save file deleted.");
        }
    }
}

   

