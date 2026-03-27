using UnityEngine;

[CreateAssetMenu(fileName = "CropData", menuName = "Farming/CropData")]
public class CropData : ScriptableObject
{
    public string cropName;
    public Sprite[] growthStageSprites;
    public float[] growthStageDuration;
    public int harvestYield = 1;
    public bool regrows = false;
}
