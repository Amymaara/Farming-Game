using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UseTool : MonoBehaviour
{
    public ToolSelector toolSelector;
    public HighlightTile highlightTile;
    public FarmManager farmManager;
    public CropManager cropManager;
    public SeedBagUI seedBagUI;

    private void Update()
    {
        if (Mouse.current == null) return;

        // Do not use tools when clicking on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PerformUseTool();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            PerformRightClickAction();
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
                Debug.Log("Seed tool used on tile: " + targetTile);
                cropManager.TryPlantSelectedSeed(targetTile);
                break;

            case ToolType.Basket:
                Debug.Log("Basket used");
                cropManager.HarvestCrop(targetTile);
                break;
            case ToolType.Sword:
                Debug.Log("Sword used");
                break;

        }
    }

    private void PerformRightClickAction()
    {
        if (toolSelector == null) return;

        switch (toolSelector.currentTool)
        {
            case ToolType.Seed:
                Debug.Log("Opening seed bag");
                if (seedBagUI != null)
                {
                    seedBagUI.Toggle();
                }
                break;
        }
    }

}
