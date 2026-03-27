using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CropData", menuName = "Farming/CropData")]
public class CropData : ScriptableObject
{
    public string cropName;
    public TileBase[] growthStageTiles;
    public float[] growthStageDurations;
    public int harvestYield = 1;
    public bool regrows = false;
}
