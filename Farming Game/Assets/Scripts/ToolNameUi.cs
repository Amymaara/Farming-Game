using TMPro;
using UnityEngine;

public class ToolNameUi : MonoBehaviour
{
    public ToolSelector toolSelector;
    public TextMeshProUGUI toolNameText;

    private ToolType lastTool;

    private void Start()
    {
        UpdateToolText();
    }

    private void Update()
    {
        if (toolSelector == null || toolNameText == null) return;

        if (toolSelector.currentTool != lastTool)
        {
            UpdateToolText();
        }
    }

    private void UpdateToolText()
    {
        lastTool = toolSelector.currentTool;
        toolNameText.text = GetReadableToolName(lastTool);
    }

    private string GetReadableToolName(ToolType tool)
    {
        switch (tool)
        {
            case ToolType.Hoe:
                return "Hoe";
            case ToolType.WateringCan:
                return "Watering Can";
            case ToolType.Seed:
                return "Seed Bag";
            case ToolType.Basket:
                return "Basket";
            case ToolType.Sword:
                return "Sword";
            case ToolType.Default:
            default:
                return "";
        }
    }
}
