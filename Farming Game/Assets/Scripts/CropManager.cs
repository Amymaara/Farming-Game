using System.Collections.Generic;
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
        if (cropTilemap.GetTile(cellPos) != null) return;

        cropTilemap.SetTile(cellPos, cropData.growthStageTiles[0]);

        PlantedCrop newCrop = new PlantedCrop
        {
            cropData = cropData,
            currentStage = 0,
            timer = cropData.growthStageDurations[0]
        };

        plantedCrops[cellPos] = newCrop;
    }

    private void UpdateGrowth()
    {
        foreach (var pair in plantedCrops)
        {
            Vector3Int pos = pair.Key;
            PlantedCrop crop = pair.Value;

            if(crop.currentStage >= crop.cropData.growthStageTiles.Length -1)
                continue;

            crop.timer -= Time.deltaTime;

            if (crop.timer <= 0f)
            {
                crop.currentStage++;
                cropTilemap.SetTile(pos, crop.cropData.growthStageTiles[crop.currentStage]);
                crop.timer = crop.cropData.growthStageDurations[crop.currentStage];

            }
        }
    }
}
