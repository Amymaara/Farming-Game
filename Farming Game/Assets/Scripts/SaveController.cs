using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    /* private string saveLocation;
    private InventoryController inventoryController;

    private void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        LoadGame();
        
    }

    private void Awake()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();

        if (player == null)
        {
            Debug.LogError("Save failed: Player not found.");
            return;
        }

        if (confiner == null)
        {
            Debug.LogError("Save failed: Confiner not found.");
            return;
        }

        if (confiner.BoundingShape2D == null)
        {
            Debug.LogError("Save failed: BoundingShape2D is null.");
            return;
        }

        SaveData saveData = new SaveData
        {
            playerPosition = player.transform.position,
            mapBoundary = confiner.BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems()
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveLocation, json);

        Debug.Log("Game saved.");
        Debug.Log(json);
    }

    public void LoadGame()
    {
        Debug.Log("LoadGame called.");

        if (!File.Exists(saveLocation))
        {
            Debug.LogWarning("No save file found.");
            return;
        }

        string json = File.ReadAllText(saveLocation);
        Debug.Log("Raw JSON: " + json);

        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        if (saveData == null)
        {
            Debug.LogError("Load failed: saveData is null.");
            return;
        }

        Debug.Log("Loaded playerPosition: " + saveData.playerPosition);
        Debug.Log("Loaded mapBoundary: " + saveData.mapBoundary);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Load failed: Player not found.");
            return;
        }

        CinemachineConfiner2D confiner = FindFirstObjectByType<CinemachineConfiner2D>();
        if (confiner == null)
        {
            Debug.LogError("Load failed: Confiner not found.");
            return;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.position = saveData.playerPosition;
            Debug.Log("Player moved with Rigidbody2D.");
        }
        else
        {
            player.transform.position = saveData.playerPosition;
            Debug.Log("Player moved with transform.");
        }

        GameObject boundaryObject = GameObject.Find(saveData.mapBoundary);
        if (boundaryObject == null)
        {
            Debug.LogError("Load failed: Boundary object not found: " + saveData.mapBoundary);
            return;
        }

        PolygonCollider2D boundary = boundaryObject.GetComponent<PolygonCollider2D>();
        if (boundary == null)
        {
            Debug.LogError("Load failed: No PolygonCollider2D on " + boundaryObject.name);
            return;
        }

        confiner.BoundingShape2D = boundary;
        confiner.InvalidateBoundingShapeCache();

        Debug.Log("Camera boundary updated to: " + boundary.name);
        Debug.Log("Load complete.");

        inventoryController.SetInventoryItems(saveData.inventorySaveData);
    }
    */

    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private Chest[] chests;

    private void Awake()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
    }

    private void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        hotbarController = FindObjectOfType<HotbarController>();
        chests = FindObjectsOfType<Chest>();
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
            handInQuestIDs = QuestController.Instance.handinQuestIDs
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveLocation, json);

        Debug.Log("Game saved.");
        Debug.Log("Saved inventory count: " + saveData.inventorySaveData.Count);
        Debug.Log(json);
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

    public void ClearSave()
    {
        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
            Debug.Log("Save file deleted.");
        }
    }
}

   

