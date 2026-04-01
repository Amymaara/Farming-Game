using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public Tilemap cropTilemap;
    public FarmManager farmManager;

    private Dictionary<Vector3Int, PlantedCrop> plantedCrops = new Dictionary<Vector3Int, PlantedCrop>();

    private void Update()
    {
        UpdateGrowth();
    }

    public void TryPlantSelectedSeed(Vector3Int cellPos)
    {
        Debug.Log("TryPlantSelectedSeed called at: " + cellPos);

        if (SeedSelector.Instance == null)
        {
            Debug.LogError("SeedSelectionController.Instance is null");
            return;
        }

        SeedData selectedSeed = SeedSelector.Instance.selectedSeed;

        if (selectedSeed == null)
        {
            Debug.LogError("Selected seed is NULL");
            return;
        }

        Debug.Log("Selected seed is: " + selectedSeed.seedName + " | ID: " + selectedSeed.seedItemID);

        if (selectedSeed.cropToPlant == null)
        {
            Debug.LogError("Selected seed has no cropToPlant assigned");
            return;
        }

        if (InventoryController.Instance == null)
        {
            Debug.LogError("InventoryController.Instance is null");
            return;
        }

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        int seedCount = itemCounts.GetValueOrDefault(selectedSeed.seedItemID);

        Debug.Log("Seed count found in inventory: " + seedCount);

        if (seedCount < 1)
        {
            Debug.LogError("No seeds left for selected seed");
            return;
        }

        bool planted = PlantCrop(cellPos, selectedSeed.cropToPlant);
        Debug.Log("PlantCrop result: " + planted);

        if (!planted)
        {
            Debug.LogWarning("PlantCrop failed, not removing seed");
            return;
        }

        InventoryController.Instance.RemoveItemsFromInventory(selectedSeed.seedItemID, 1);
        InventoryController.Instance.RebuildItemCounts();

        Debug.Log("Successfully planted and removed 1 seed");
    }
    public bool PlantCrop(Vector3Int cellPos, CropData cropData)
    {
        if (cropData == null)
        {
            Debug.LogWarning("No crop data assigned.");
            return false;
        }

        if (cropData.growthStageTiles == null || cropData.growthStageTiles.Length == 0)
        {
            Debug.LogWarning("Crop has no growth stage tiles.");
            return false;
        }

        if (cropData.growthStageDurations == null || cropData.growthStageDurations.Length == 0)
        {
            Debug.LogWarning("Crop has no growth durations.");
            return false;
        }

        if (!farmManager.IsFarmable(cellPos))
        {
            Debug.LogWarning("Not a farmable tile.");
            return false;
        }

        TileBase soilTile = farmManager.farmTilemap.GetTile(cellPos);
        Debug.Log("Soil tile at target: " + (soilTile != null ? soilTile.name : "NULL"));

        if (soilTile != farmManager.tilledTile && soilTile != farmManager.wateredTile)
        {
            Debug.LogWarning("Tile must be tilled or watered before planting.");
            return false;
        }

        if (cropTilemap.GetTile(cellPos) != null)
        {
            Debug.LogWarning("Crop already planted here.");
            return false;
        }

        cropTilemap.SetTile(cellPos, cropData.growthStageTiles[0]);

        PlantedCrop newCrop = new PlantedCrop
        {
            cropData = cropData,
            currentStage = 0,
            timer = cropData.growthStageDurations.Length > 0 ? cropData.growthStageDurations[0] : 1f
        };

        plantedCrops[cellPos] = newCrop;

        Debug.Log("Planted " + cropData.cropName);
        return true;

    }

    private void UpdateGrowth()
    {
        List<Vector3Int> positions = new List<Vector3Int>(plantedCrops.Keys);

        foreach (Vector3Int pos in positions)
        {
            PlantedCrop crop = plantedCrops[pos];

            if (crop.currentStage >= crop.cropData.growthStageTiles.Length - 1)
                continue;

            TileBase soilTile = farmManager.farmTilemap.GetTile(pos);

            // Only grow if watered
            if (soilTile != farmManager.wateredTile)
                continue;

            crop.timer -= Time.deltaTime;

            if (crop.timer <= 0f)
            {
                crop.currentStage++;

                cropTilemap.SetTile(pos, crop.cropData.growthStageTiles[crop.currentStage]);

                Debug.Log(crop.cropData.cropName + " advanced to stage " + crop.currentStage);

                // Remove watered state after one growth stage
                farmManager.farmTilemap.SetTile(pos, farmManager.tilledTile);

                if (crop.currentStage < crop.cropData.growthStageDurations.Length)
                {
                    crop.timer = crop.cropData.growthStageDurations[crop.currentStage];
                }
            }
        }
    }

    public bool HarvestCrop(Vector3Int cellPos)
    {
        Debug.Log("Trying to harvest at: " + cellPos);

        if (!plantedCrops.ContainsKey(cellPos))
        {
            Debug.Log("Nothing planted here.");
            return false;
        }

        PlantedCrop crop = plantedCrops[cellPos];

        Debug.Log("Found crop: " + crop.cropData.cropName + " | stage: " + crop.currentStage);

        bool isFullyGrown = crop.currentStage >= crop.cropData.growthStageTiles.Length - 1;

        if (!isFullyGrown)
        {
            Debug.Log("Crop is not fully grown yet.");
            return false;
        }

        int harvestAmount = crop.cropData.harvestYield > 0 ? crop.cropData.harvestYield : 1;
        int harvestItemID = crop.cropData.harvestItemID;

        Debug.Log("Harvest item ID: " + harvestItemID + " | amount: " + harvestAmount);

        if (InventoryController.Instance == null)
        {
            Debug.LogWarning("InventoryController.Instance is null");
            return false;
        }

        ItemDictionary itemDictionary = FindObjectOfType<ItemDictionary>();
        if (itemDictionary == null)
        {
            Debug.LogWarning("ItemDictionary not found");
            return false;
        }

        GameObject itemPrefab = itemDictionary.GetItemPrefab(harvestItemID);
        if (itemPrefab == null)
        {
            Debug.LogWarning("No item prefab found for harvest item ID: " + harvestItemID);
            return false;
        }

        Debug.Log("Found prefab: " + itemPrefab.name);

        bool added = InventoryController.Instance.AddItem(itemPrefab, harvestAmount);
        Debug.Log("AddItem returned: " + added);

        if (!added)
        {
            Debug.LogWarning("Harvest failed because item could not be added to inventory.");
            return false;
        }

        cropTilemap.SetTile(cellPos, null);
        plantedCrops.Remove(cellPos);

        if (QuestController.Instance != null)
        {
            QuestController.Instance.RegisterCollectedItem(harvestItemID, harvestAmount);
        }

        Debug.Log("Harvested " + crop.cropData.cropName + " x" + harvestAmount);
        return true;
    }

}
