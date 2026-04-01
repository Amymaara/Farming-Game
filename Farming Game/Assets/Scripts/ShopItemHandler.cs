using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemHandler : MonoBehaviour, IPointerClickHandler
{
    private bool isShopitem;
    public Slot originalInventorySlot;

    public void Initiliase(bool shopItem) => isShopitem = shopItem;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isShopitem)
            {
                BuyItem();
            }
            else
            {
                SellItem();
            }
        }
    }

    private void BuyItem()
    {
        Debug.Log("BuyItem called");

        Item item = GetComponent<Item>();
        ShopSlot slot = GetComponentInParent<ShopSlot>();

        if (item == null)
        {
            Debug.LogError("BuyItem failed: Item is null");
            return;
        }

        if (slot == null)
        {
            Debug.LogError("BuyItem failed: ShopSlot is null");
            return;
        }

        Debug.Log("Trying to buy item: " + item.name + " | ID: " + item.ID + " | Price: " + slot.itemPrice);

        if (CurrencyController.Instance == null)
        {
            Debug.LogError("BuyItem failed: CurrencyController.Instance is null");
            return;
        }

        Debug.Log("Player gold: " + CurrencyController.Instance.GetGold());

        if (CurrencyController.Instance.GetGold() < slot.itemPrice)
        {
            Debug.Log("Not enough gold!");
            return;
        }

        ItemDictionary dictionary = FindObjectOfType<ItemDictionary>();
        if (dictionary == null)
        {
            Debug.LogError("BuyItem failed: ItemDictionary not found");
            return;
        }

        GameObject itemPrefab = dictionary.GetItemPrefab(item.ID);
        if (itemPrefab == null)
        {
            Debug.LogError("BuyItem failed: No prefab found for item ID " + item.ID);
            return;
        }

        Debug.Log("Found prefab to buy: " + itemPrefab.name);

        if (InventoryController.Instance == null)
        {
            Debug.LogError("BuyItem failed: InventoryController.Instance is null");
            return;
        }

        bool added = InventoryController.Instance.AddItem(itemPrefab, 1);
        Debug.Log("Inventory AddItem returned: " + added);

        if (added)
        {
            CurrencyController.Instance.SpendGold(slot.itemPrice);
            ShopController.Instance.RefreshInventoryDisplay();
            bool removed = ShopController.Instance.RemoveItemFromShop(item.ID, 1);
            Debug.Log("RemoveItemFromShop returned: " + removed);
        }
        else
        {
            Debug.Log("Inventory full!");
        }
    }

    private void SellItem()
    {
        Item item = GetComponent<Item>();
        ShopSlot slot = GetComponentInParent<ShopSlot>();

        if (item == null || slot == null || originalInventorySlot == null) return;
        if (originalInventorySlot.currentItem == null) return;

        Item invItem = originalInventorySlot.currentItem.GetComponent<Item>();
        if (invItem == null) return;

        if (invItem.quantity > 1)
        {
            invItem.RemoveFromStack(1);
        }
        else
        {
            Destroy(originalInventorySlot.currentItem);
            originalInventorySlot.currentItem = null;
        }

        InventoryController.Instance.RebuildItemCounts();
        CurrencyController.Instance.AddGold(slot.itemPrice);
        ShopController.Instance.RefreshInventoryDisplay();
        ShopController.Instance.AddItemToShop(item.ID, 1);
    }
}
