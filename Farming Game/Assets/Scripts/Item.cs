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

        Debug.Log("You are using an item");
    }

    public virtual bool ShowPopUp()
    {
        if (isBeingPickedUp) return false;
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

                Destroy(gameObject);
                return true;
            }
            else
            {
                if (col != null) col.enabled = true;
                isBeingPickedUp = false;
                return false;
            }
        }
        else
        {
            if (col != null) col.enabled = true;
            isBeingPickedUp = false;
            return false;
        }

    }


}
