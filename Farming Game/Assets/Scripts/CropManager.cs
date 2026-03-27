using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CropManager : MonoBehaviour
{
    public Tilemap cropTilemap;
    public FarmManager farmManager;

    private Dictionary<Vector3Int, CropData> plantedCrops = new Dictionary<Vector3Int, CropData>();

    public void PlantCrop(Vector3Int cellPos, CropData cropData)
    {
        cropTilemap.SetTile(cellPos, cropData.growthStageTiles[0]);
        plantedCrops[cellPos] = cropData;

    }

}
