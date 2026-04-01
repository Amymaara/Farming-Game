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
        if (SeedSelector.Instance == null)
        {
            Debug.LogWarning("No SeedSelectionController found.");
            return;
        }

        SeedData selectedSeed = SeedSelector.Instance.selectedSeed;

        if (selectedSeed == null)
        {
            Debug.Log("No seed selected.");
            return;
        }

        if (selectedSeed.cropToPlant == null)
        {
            Debug.LogWarning("Selected seed has no crop assigned.");
            return;
        }

        if (InventoryController.Instance == null)
        {
            Debug.LogWarning("InventoryController.Instance is null");
            return;
        }

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        if (itemCounts.GetValueOrDefault(selectedSeed.seedItemID) < 1)
        {
            Debug.Log("No seeds left for: " + selectedSeed.seedName);
            return;
        }

        // plant using your existing method
        PlantCrop(cellPos, selectedSeed.cropToPlant);

        // remove one seed after planting
        InventoryController.Instance.RemoveItemsFromInventory(selectedSeed.seedItemID, 1);
        InventoryController.Instance.RebuildItemCounts();

        Debug.Log("Planted using selected seed: " + selectedSeed.seedName);
    }
    public void PlantCrop(Vector3Int cellPos, CropData cropData)
    {
        if (cropData.growthStageTiles == null || cropData.growthStageTiles.Length == 0)
        {
            Debug.LogWarning("Crop has no growth stage tiles.");
            return;
        }

        if (cropData.growthStageDurations == null || cropData.growthStageDurations.Length == 0)
        {
            Debug.LogWarning("Crop has no growth durations.");
            return;
        }

        if (cropData == null)
        {
            Debug.LogWarning("No crop data assigned.");
            return;
        }

        if (!farmManager.IsFarmable(cellPos))
        {
            Debug.Log("Not a farmable tile.");
            return;
        }

        TileBase soilTile = farmManager.farmTilemap.GetTile(cellPos);

        if (soilTile != farmManager.tilledTile && soilTile != farmManager.wateredTile)
        {
            Debug.Log("Tile must be tilled or watered before planting.");
            return;
        }

        if (cropTilemap.GetTile(cellPos) != null)
        {
            Debug.Log("Crop already planted here.");
            return;
        }

        cropTilemap.SetTile(cellPos, cropData.growthStageTiles[0]);

        PlantedCrop newCrop = new PlantedCrop
        {
            cropData = cropData,
            currentStage = 0,
            timer = cropData.growthStageDurations[0]
        };

        plantedCrops[cellPos] = newCrop;

        Debug.Log("Planted " + cropData.cropName);

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
