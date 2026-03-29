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
    public void PlantCrop(Vector3Int cellPos, CropData cropData)
    {
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

  /* public bool HarvestCrop(Vector3Int cellPos)
    {
        if (!plantedCrops.ContainsKey(cellPos))
        {
            Debug.Log("nothing planted");
            return false;
        }
    }

    PlantedCrop crop = plantedCrops[cellPos];

    bool isFullyGrown = crop.currentStage >= crop.CropData.growthStageTiles.Length - 1;
  */
}
