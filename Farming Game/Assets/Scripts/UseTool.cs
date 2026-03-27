using UnityEngine;
using UnityEngine.InputSystem;

public class UseTool : MonoBehaviour
{
    public ToolSelector toolSelector;
    public HighlightTile highlightTile;
    public FarmManager farmManager;
    public CropManager cropManager;
    public CropData selectedCrop;

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PerformUseTool();
        }
    }

    private void PerformUseTool()
    {
        if (toolSelector == null || highlightTile == null) return;

        Vector3Int targetTile = highlightTile.currentTile;

        switch (toolSelector.currentTool)
        {
            case ToolType.Default:
                Debug.Log("No tool selected.");
                break;

            case ToolType.Hoe:
                Debug.Log("Hoe used");
                farmManager.HoeTile(targetTile);
                break;

            case ToolType.WateringCan:
                farmManager.WaterTile(targetTile);
                break;

            case ToolType.Seed:
                Debug.Log("Seed planted");
                cropManager.PlantCrop(targetTile, selectedCrop);
                break;

            case ToolType.Basket:
                Debug.Log("Basket used");
                // PickupItem(targetCell);
                break;
            case ToolType.Sword:
                Debug.Log("Sword used");
                break;

        }
    }

}
