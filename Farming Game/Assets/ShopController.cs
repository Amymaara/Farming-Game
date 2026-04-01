using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance;

    [Header("UI")]
    public GameObject shopPanel;
    public Transform shopInventoryGrid, playerInventoryGrid;
    public GameObject shopSlotPrefab;
    public TMP_Text playerMoneyText, shopTitleText;

    private ItemDictionary itemDictionary;
    private ShopNPC currentShop;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        shopPanel.SetActive(false);
        if (CurrencyController.Instance != null)
        {
            CurrencyController.Instance.onGoldChanged += UpdateMoneyDisplay;
        }

    }

    private void UpdateMoneyDisplay(int amount)
    {
        if(playerMoneyText != null) 
            playerMoneyText.text = amount.ToString();
    }

    public void OpenShop(ShopNPC shop)
    {
        currentShop = shop;
        shopPanel.SetActive(true);
        if (shopTitleText != null) shopTitleText.text = shop.shopKeeperName + "'s Shop";
        RefreshShopDisplay();
        RefreshInventoryDisplay();
        PauseController.SetPause(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
        currentShop = null;
        PauseController.SetPause(false);
    }

    public void RefreshShopDisplay()
    {
        if(currentShop == null) return;
        foreach (Transform child in shopInventoryGrid) Destroy(child.gameObject);

        foreach (var stockItem in currentShop.GetCurrentStock())
        {
            if (stockItem.quantity <= 0) continue;

            CreateShopSlot(shopInventoryGrid, stockItem.itemID, stockItem.quantity, true);
        }
    }

    public void RefreshInventoryDisplay()
    {
        Debug.Log("RefreshInventoryDisplay CALLED");

        if (InventoryController.Instance == null)
        {
            Debug.LogError("InventoryController.Instance is null");
            return;
        }

        if (playerInventoryGrid == null)
        {
            Debug.LogError("playerInventoryGrid is null");
            return;
        }

        foreach (Transform child in playerInventoryGrid)
            Destroy(child.gameObject);

        List<InventorySaveData> inventoryItems = InventoryController.Instance.GetInventoryItems();
        Debug.Log("Inventory items found: " + inventoryItems.Count);

        foreach (InventorySaveData data in inventoryItems)
        {
            Debug.Log("Creating player slot for itemID: " + data.itemID + " qty: " + data.quantity);
            CreateShopSlot(playerInventoryGrid, data.itemID, data.quantity, false);
        }

        Debug.Log("Player inventory grid child count AFTER build: " + playerInventoryGrid.childCount);
    }

    private void CreateShopSlot(Transform grid, int itemID, int quantity, bool isShop, Slot originalSlot = null)
    {
        Debug.Log("CreateShopSlot called | itemID: " + itemID + " | qty: " + quantity + " | isShop: " + isShop);

        if (grid == null)
        {
            Debug.LogError("grid is null");
            return;
        }

        if (itemDictionary == null)
        {
            Debug.LogError("itemDictionary is null");
            return;
        }

        if (shopSlotPrefab == null)
        {
            Debug.LogError("shopSlotPrefab is null");
            return;
        }

        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);
        if (itemPrefab == null)
        {
            Debug.LogError("No item prefab found for itemID: " + itemID);
            return;
        }

        GameObject slotObj = Instantiate(shopSlotPrefab, grid);
        if (slotObj == null)
        {
            Debug.LogError("slotObj failed to instantiate");
            return;
        }

        ShopSlot slot = slotObj.GetComponent<ShopSlot>();
        if (slot == null)
        {
            Debug.LogError("ShopSlot component missing on shopSlotPrefab root");
            return;
        }

        GameObject itemInstance = Instantiate(itemPrefab, slotObj.transform);
        if (itemInstance == null)
        {
            Debug.LogError("itemInstance failed to instantiate");
            return;
        }

        RectTransform rect = itemInstance.GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogError("Item prefab has no RectTransform: " + itemPrefab.name);
            return;
        }

        rect.anchoredPosition = Vector2.zero;

        Item item = itemInstance.GetComponent<Item>();
        if (item == null)
        {
            Debug.LogError("Item component missing on prefab: " + itemPrefab.name);
            return;
        }

        item.quantity = quantity;
        item.UpdateQuantityDisplay();

        int price = isShop ? item.buyPrice : item.GetSellPrice();

        slot.isShopSlot = isShop;
        slot.SetItem(itemInstance, price);
    }
}
