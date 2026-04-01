using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public Tilemap cropTilemap;
    public FarmManager farmManager;
    public GameObject waterIconPrefab;

    private Dictionary<Vector3Int, PlantedCrop> plantedCrops = new Dictionary<Vector3Int, PlantedCrop>();

    private void Update()
    {
        UpdateGrowth();
        UpdateWaterIcons();
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
            timer = cropData.growthStageDurations[0],
            waterIcon = null
        };

        if (waterIconPrefab != null)
        {
            Vector3 worldPos = cropTilemap.GetCellCenterWorld(cellPos) + new Vector3(0f, 0.6f, 0f);
            newCrop.waterIcon = Instantiate(waterIconPrefab, worldPos, Quaternion.identity);
            newCrop.waterIcon.SetActive(true);
        }

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

            if (soilTile != farmManager.wateredTile)
                continue;

            crop.timer -= Time.deltaTime;

            if (crop.timer <= 0f)
            {
                crop.currentStage++;
                cropTilemap.SetTile(pos, crop.cropData.growthStageTiles[crop.currentStage]);

                bool isNowFullyGrown = crop.currentStage >= crop.cropData.growthStageTiles.Length - 1;

                if (isNowFullyGrown && TutorialProgress.Instance != null && !TutorialProgress.Instance.firstCropHarvested)
                {
                    if (TutorialPopupManager.Instance != null)
                    {
                        TutorialPopupManager.Instance.ShowPopup("Use the basket to harvest fully grown crops.");
                        TutorialProgress.Instance.firstCropHarvested = true;
                    }
                }

                farmManager.farmTilemap.SetTile(pos, farmManager.tilledTile);

                if (crop.currentStage < crop.cropData.growthStageDurations.Length)
                {
                    crop.timer = crop.cropData.growthStageDurations[crop.currentStage];
                }
            }
        }
    }

    private void UpdateWaterIcons()
    {
        foreach (var pair in plantedCrops)
        {
            Vector3Int pos = pair.Key;
            PlantedCrop crop = pair.Value;

            if (crop.waterIcon == null)
                continue;

            crop.waterIcon.transform.position = cropTilemap.GetCellCenterWorld(pos) + new Vector3(0f, 0.6f, 0f);

            TileBase soilTile = farmManager.farmTilemap.GetTile(pos);
            bool isWatered = soilTile == farmManager.wateredTile;
            bool isFullyGrown = crop.currentStage >= crop.cropData.growthStageTiles.Length - 1;

            crop.waterIcon.SetActive(!isWatered && !isFullyGrown);
        }
    }

    public bool HarvestCrop(Vector3Int cellPos)
    {
        if (!plantedCrops.ContainsKey(cellPos))
        {
            Debug.Log("Nothing planted here.");
            return false;
        }

        PlantedCrop crop = plantedCrops[cellPos];
        bool isFullyGrown = crop.currentStage >= crop.cropData.growthStageTiles.Length - 1;

        if (!isFullyGrown)
        {
            Debug.Log("Crop is not fully grown yet.");
            return false;
        }

        int harvestAmount = crop.cropData.harvestYield > 0 ? crop.cropData.harvestYield : 1;
        int harvestItemID = crop.cropData.harvestItemID;

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

        bool added = InventoryController.Instance.AddItem(itemPrefab, harvestAmount);

        if (!added)
        {
            Debug.LogWarning("Harvest failed because item could not be added to inventory.");
            return false;
        }


        if (crop.waterIcon != null)
        {
            Destroy(crop.waterIcon);
        }

        cropTilemap.SetTile(cellPos, null);
        plantedCrops.Remove(cellPos);

        if (QuestController.Instance != null)
        {
            QuestController.Instance.RegisterCollectedItem(harvestItemID, harvestAmount);
        }

        return true;
    }

    public void TryPlantSelectedSeed(Vector3Int cellPos)
    {
        Debug.Log("TryPlantSelectedSeed called at: " + cellPos);

        if (SeedSelector.Instance == null)
        {
            Debug.LogError("SeedSelector.Instance is null");
            return;
        }

        SeedData selectedSeed = SeedSelector.Instance.selectedSeed;

        if (selectedSeed == null)
        {
            Debug.LogWarning("No seed selected.");
            return;
        }

        if (selectedSeed.cropToPlant == null)
        {
            Debug.LogError("Selected seed has no cropToPlant assigned.");
            return;
        }

        if (InventoryController.Instance == null)
        {
            Debug.LogError("InventoryController.Instance is null");
            return;
        }

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        int seedCount = itemCounts.GetValueOrDefault(selectedSeed.seedItemID);

        if (seedCount < 1)
        {
            Debug.LogWarning("No seeds left for selected seed.");
            return;
        }

        bool planted = PlantCrop(cellPos, selectedSeed.cropToPlant);

        if (!planted)
        {
            Debug.LogWarning("Planting failed, seed not removed.");
            return;
        }

        InventoryController.Instance.RemoveItemsFromInventory(selectedSeed.seedItemID, 1);
        InventoryController.Instance.RebuildItemCounts();

        if (TutorialProgress.Instance != null && !TutorialProgress.Instance.firstCropWatered)
        {
            TutorialProgress.Instance.firstCropWatered = true;
            TutorialPopupManager.Instance.ShowPopup("Water crops to help them grow.");
        }

        Debug.Log("Successfully planted and removed 1 seed.");
    }

}
