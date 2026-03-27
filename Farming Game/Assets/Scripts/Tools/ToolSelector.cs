using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToolSelector : MonoBehaviour
{
    public ToolType currentTool = ToolType.Default;
   

    [Header("Ui for tools")]
    public Image toolSlotIcon;
    public Sprite defaultSprite;
    public Sprite hoeSprite;
    public Sprite wateringCanSprite;
    public Sprite seedPacketSprite;
    public Sprite basketSprite;
    public Sprite swordSprite;

    private ToolType[] tools =
    {
     ToolType.Default,
     ToolType.Hoe,
     ToolType.WateringCan,
     ToolType.Seed,
     ToolType.Basket,   
     ToolType.Sword,
    };

    private int currentToolIndex = 0;
    private float scrollCooldown = 0.15f;
    private float lastScrollTime;

    private void Start()
    {
        UpdateToolUI();
    }

    private void Update()
    {
        HandleScrollInput();
    }

    private void HandleScrollInput()
    {
        if (Mouse.current == null) return;
        if (Time.time < lastScrollTime + scrollCooldown) return;

        float scrollY = Mouse.current.scroll.ReadValue().y;

        if (scrollY > 0)
        {
            currentToolIndex++;
            if (currentToolIndex >= tools.Length)
                currentToolIndex = 0;

            SetTool(tools[currentToolIndex]);
            lastScrollTime = Time.time;
        }
        else if (scrollY < 0)
        {
            currentToolIndex--;
            if (currentToolIndex < 0)
                currentToolIndex = tools.Length - 1;

            SetTool(tools[currentToolIndex]);
            lastScrollTime = Time.time;
        }
    }

    private void SetTool(ToolType newTool)
    {
        currentTool = newTool;
        UpdateToolUI();
        Debug.Log("Selected tool is: + currentTool");
    }

    private void UpdateToolUI()
    {
        if (toolSlotIcon == null) return;

        switch (currentTool)
        {
            case ToolType.Default:
                toolSlotIcon.sprite = defaultSprite;
            break;

            case ToolType.Hoe:
                toolSlotIcon.sprite = hoeSprite; 
            break;

            case ToolType.WateringCan:
                toolSlotIcon.sprite = wateringCanSprite;
            break;

            case ToolType.Seed:
                toolSlotIcon.sprite = seedPacketSprite;
                break;

            case ToolType.Basket:
                toolSlotIcon.sprite = basketSprite;
            break;

            case ToolType.Sword:
                toolSlotIcon.sprite = swordSprite;
            break;

        }
    }

}
