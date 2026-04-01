using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "SeedData", menuName = "Farming/SeedData")]
public class SeedData : ScriptableObject
{
    public int seedItemID;
    public string seedName;
    public Sprite seedIcon;

    public CropData cropToPlant;
}
