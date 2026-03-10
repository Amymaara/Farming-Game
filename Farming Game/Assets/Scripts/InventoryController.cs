using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    /* private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    private void Awake()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        InializeEmptyInventory();
    }
    private void Start()
    {

        /*for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
            }
        }
    }

    private void InializeEmptyInventory()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("InventoryPanel is not assigned.");
            return;
        }

        if (slotPrefab == null)
        {
            Debug.LogError("SlotPrefab is not assigned.");
            return;
        }

        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {
        //look empty slot
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }

        }

        // animations etc here
        Debug.Log("inv  full"); 
        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData { itemID = item.ID, slotIndex = slotTransform.GetSiblingIndex() });
            }
        }

        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        InializeEmptyInventory();

        if (itemDictionary == null)
        {
            Debug.Log("itemDictionary null");
            return;
        }

        if (inventorySaveData == null)
        {
            Debug.Log("no inv save data found");
        }


        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
            }
        }
    }
 */

    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    private List<Slot> slots = new List<Slot>();

    private void Awake()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();
        CreateSlots();
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

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Slot slot in slots)
        {
            if (slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
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
                        slotIndex = slot.slotIndex
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
