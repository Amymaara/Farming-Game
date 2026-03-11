using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 10;

    private ItemDictionary itemDictionary;
    private Key[] hotbarKeys;
    private List<Slot> slots = new List<Slot>();

    private void Awake()
    {
        itemDictionary = FindObjectOfType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }

        CreateSlots();
    }

    private void CreateSlots()
    {
        slots.Clear();

        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, hotbarPanel.transform);
            Slot slot = slotObj.GetComponent<Slot>();
            slot.slotIndex = i;
            slot.currentItem = null;
            slots.Add(slot);
        }
    }

    private void ClearItemsOnly()
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

    private void Update()
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current != null && Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                UseItemInSlot(i);
            }
        }
    }

    private void UseItemInSlot(int index)
    {
        if (index < 0 || index >= slots.Count) return;

        Slot slot = slots[index];

        if (slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            if (item != null)
            {
                item.UseItem();
            }
        }
    }

    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();

        foreach (Slot slot in slots)
        {
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();

                if (item != null)
                {
                    hotbarData.Add(new InventorySaveData
                    {
                        itemID = item.ID,
                        slotIndex = slot.slotIndex
                    });
                }
            }
        }

        return hotbarData;
    }

    public void SetHotbarItems(List<InventorySaveData> hotbarSaveData)
    {
        ClearItemsOnly();

        if (hotbarSaveData == null)
        {
            Debug.LogWarning("No hotbar save data found.");
            return;
        }

        foreach (InventorySaveData data in hotbarSaveData)
        {
            if (data.slotIndex < 0 || data.slotIndex >= slots.Count)
            {
                Debug.LogWarning("Invalid hotbar slot index: " + data.slotIndex);
                continue;
            }

            GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);

            if (itemPrefab != null)
            {
                Slot slot = slots[data.slotIndex];

                GameObject item = Instantiate(itemPrefab, slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;

                Debug.Log("Loaded hotbar item ID " + data.itemID + " into slot " + data.slotIndex);
            }
            else
            {
                Debug.LogWarning("No prefab found for item ID: " + data.itemID);
            }
        }
    }
}
