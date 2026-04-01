using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).

public class InventoryController : MonoBehaviour
{
<<<<<<< HEAD
    /*  private ItemDictionary itemDictionary;

      public GameObject inventoryPanel;
      public GameObject slotPrefab;
      public int slotCount;

      private List<Slot> slots = new List<Slot>();

      public static InventoryController Instance {  get; private set; }
      Dictionary<int, int> itemsCountCache = new();
      public event Action OnInventoryChanged; // tells quest system / other scripts what it needs to know (fetch/ collect quests)
      private void Awake()
      {
          itemDictionary = FindObjectOfType<ItemDictionary>();
          CreateSlots();
          RebuildItemCounts();

          if (Instance != null && Instance != this)
          {
              Destroy(gameObject);
              return;
          }

          Instance = this;
      }

      public void RebuildItemCounts()
      {
          itemsCountCache.Clear();

          foreach (Transform slotTr in inventoryPanel.transform)
          {
              Slot slot = slotTr.GetComponent<Slot>();
              if (slot.currentItem != null)
              {
                  Item item = slot.currentItem.GetComponent<Item>();
                  if (item != null)
                  {
                      itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                      Debug.Log($"Counting item {item.name} | ID: {item.ID} | Qty: {item.quantity}");

                  }
              }
          }

          OnInventoryChanged?.Invoke();
      }

      public Dictionary<int, int> GetItemCounts() => itemsCountCache;

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
                      RebuildItemCounts();
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
                  RebuildItemCounts();
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
                  if (itemComponent != null && data.quantity > 1)
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

          RebuildItemCounts();
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

      public void RemoveItemsFromInventory(int itemID, int amountToRemove)
      {
          foreach (Transform slotTransform in inventoryPanel.transform)
          {
              if (amountToRemove <= 0) break;

              Slot slot = slotTransform.GetComponent<Slot>();
              if (slot?.currentItem?.GetComponent<Item>() is Item item && item.ID == itemID)
              {
                  int removed = Mathf.Min(amountToRemove, item.quantity);
                  item.RemoveFromStack(removed);
                  amountToRemove -= removed;

                  if (item.quantity == 0)
                  {
                      Destroy(slot.currentItem);
                      slot.currentItem = null;
                  }
              }
          }

          RebuildItemCounts();
      } */
=======
>>>>>>> Farming-Mechanics

    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    private List<Slot> slots = new List<Slot>();

    public static InventoryController Instance { get; private set; }
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        itemDictionary = FindObjectOfType<ItemDictionary>();
        CreateSlots();
        RebuildItemCounts();
    }

    public List<Slot> GetSlots()
    {
        return slots;
    }

    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();

        foreach (Transform slotTr in inventoryPanel.transform)
        {
            Slot slot = slotTr.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                    Debug.Log($"Counting item {item.name} | ID: {item.ID} | Qty: {item.quantity}");
                }
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public Dictionary<int, int> GetItemCounts() => itemsCountCache;

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
                    slotItem.AddToStack(amount);
                    RebuildItemCounts();
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

                RectTransform rect = newItem.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                }

                Item newItemComponent = newItem.GetComponent<Item>();
                if (newItemComponent != null)
                {
                    newItemComponent.quantity = amount;
                    newItemComponent.UpdateQuantityDisplay();
                }

                ItemDragHandler dragHandler = newItem.GetComponent<ItemDragHandler>();
                if (dragHandler != null)
                {
                    dragHandler.enabled = true;
                }

                CanvasGroup canvasGroup = newItem.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.alpha = 1f;
                }

                slot.currentItem = newItem;

                Debug.Log("Item added to slot " + slot.slotIndex);
                RebuildItemCounts();
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
                RectTransform rect = item.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                    rect.localScale = Vector3.one;
                    rect.localRotation = Quaternion.identity;
                }

                Item itemComponent = item.GetComponent<Item>();
                if (itemComponent != null)
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

        RebuildItemCounts();
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

    public void RemoveItemsFromInventory(int itemID, int amountToRemove)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            if (amountToRemove <= 0) break;

            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot?.currentItem?.GetComponent<Item>() is Item item && item.ID == itemID)
            {
                int removed = Mathf.Min(amountToRemove, item.quantity);
                item.RemoveFromStack(removed);
                amountToRemove -= removed;

                if (item.quantity <= 0)
                {
                    Destroy(slot.currentItem);
                    slot.currentItem = null;
                }
            }
        }

        RebuildItemCounts();
    }
}
