using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using static PlayerMovement;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).
public class Item : MonoBehaviour
{

    public int ID;
    public string Name;
    public int quantity = 1;

    private TMP_Text quantityText;
    private bool isBeingPickedUp = false;

    private void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>(true);
        UpdateQuantityDisplay();
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public GameObject CloneItem(int newQuantity)
    {
        GameObject clone = Instantiate(gameObject);
        Item cloneItem = clone.GetComponent<Item>();
        cloneItem.quantity = newQuantity;
        cloneItem.UpdateQuantityDisplay();
        return clone;
    }

    public void UpdateQuantityDisplay()
    {
       if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }

    }

    public virtual void UseItem()
    {

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        FarmTile farmTile = null;
        farmTile = FindFirstObjectByType<FarmTile>();

        if (player == null)
        {
            Debug.LogWarning("No FarmingToolController found!");
            return;
        }

        switch (ID)
        {
            case 100:
                player.currentTool = ToolType.hoe;
                farmTile.PloughSoil();
                break;

            case 101:
                player.currentTool = ToolType.wateringCan;
                Debug.Log("Equipped Watering Can");
                break;

            case 102:
                player.currentTool = ToolType.seeds;
                Debug.Log("Equipped Seeds");
                break;

            case 103:
                player.currentTool = ToolType.basket;
                Debug.Log("Equipped Basket");
                break;

            default:
                Debug.Log("This item is not a tool.");
                break;
        }
    }

    public virtual void ShowPopUp()
    {
        if (isBeingPickedUp) return;
        isBeingPickedUp = true;

        Sprite itemIcon = null;

        Image image = GetComponent<Image>();
        if (image != null)
        {
            itemIcon = image.sprite;
        }
        else
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                itemIcon = spriteRenderer.sprite;
            }
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (InventoryController.Instance != null)
        {
            bool added = InventoryController.Instance.AddItem(gameObject, quantity);

            if (added)
            {
                if (ItemPickupUIController.Instance != null)
                {
                    ItemPickupUIController.Instance.ShowItemPickup(Name, itemIcon);
                }

                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                if (col != null) col.enabled = true;
                isBeingPickedUp = false;
            }
        }
        else
        {
            if (col != null) col.enabled = true;
            isBeingPickedUp = false;
        }
    }
}
