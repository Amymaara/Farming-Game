using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log("Using item " + Name);
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
