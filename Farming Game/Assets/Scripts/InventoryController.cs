using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    private List<Slot> slots = new List<Slot>();

    public static InventoryController Instance {  get; private set; }
    private void Awake()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        CreateSlots();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void CreateSlots()
    {
        slots.Clear();

        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, inventoryPanel.transform);
            Slot slot = slotObj.GetComponent<Slot>();
            slot.slotIndex = i;
            slot.currentItem = null;
            slots.Add(slot);
        }
    }

    public bool AddItem(GameObject itemPrefab, int amount = 1)
    {
        Item itemToAdd = itemPrefab.GetComponent<Item>();
        if (itemToAdd == null) return false;

        // Check if we already have this item type in inventory
        foreach (Slot slot in slots)
        {
            if (slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();
                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    // Same item stack
                    slotItem.AddToStack(amount);
                    return true;
                }
            }
        }

        // Look for empty slot
        foreach (Slot slot in slots)
        {
            if (slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                Item newItemComponent = newItem.GetComponent<Item>();
                if (newItemComponent != null)
                {
                    newItemComponent.quantity = amount;
                    newItemComponent.UpdateQuantityDisplay();
                }

                slot.currentItem = newItem;

                Debug.Log("Item added to slot " + slot.slotIndex);
                return true;
            }
        }

        Debug.Log("Inventory full");
        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();

        foreach (Slot slot in slots)
        {
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();

                if (item != null)
                {
                    invData.Add(new InventorySaveData
                    {
                        itemID = item.ID,
                        slotIndex = slot.slotIndex,
                        quantity = item.quantity,
                    });
                }
            }
        }

        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        ClearItemsOnly();

        if (inventorySaveData == null)
        {
            Debug.LogWarning("No inventory save data found.");
            return;
        }

        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < 0 || data.slotIndex >= slots.Count)
            {
                Debug.LogWarning("Invalid slot index: " + data.slotIndex);
                continue;
            }

            GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

            if (itemPrefab != null)
            {
                Slot slot = slots[data.slotIndex];

                GameObject item = Instantiate(itemPrefab, slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                Item itemComponent = item.GetComponent<Item>();
                if (itemComponent != null & data.quantity > 1)
                {
                    itemComponent.quantity = data.quantity;
                    itemComponent.UpdateQuantityDisplay();
                }
                slot.currentItem = item;

                Debug.Log("Loaded item ID " + data.itemID + " into slot " + data.slotIndex);
            }
            else
            {
                Debug.LogWarning("No prefab found for item ID: " + data.itemID);
            }
        }
    }

    public void ClearItemsOnly()
    {
        foreach (Slot slot in slots)
        {
            if (slot.currentItem != null)
            {
                Destroy(slot.currentItem);
                slot.currentItem = null;
            }
        }
    }


}
