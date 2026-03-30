using UnityEngine;
using UnityEngine.EventSystems;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).
public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;

    public float minDropDistance = 2f;
    public float maxDropDistance = 3f;

    private InventoryController inventoryController;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController = InventoryController.Instance;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
      originalParent = transform.parent; // Save OG parent
        transform.SetParent(transform.root); // above other canvas
        canvasGroup.blocksRaycasts = false; 
        canvasGroup.alpha = 0.6f; // semi transparent during dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; //Enables raycasts
        canvasGroup.alpha = 1f; //No longer transparent

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //Slot where item dropped
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot == originalSlot)
        {
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            return;
        }

        if (dropSlot != null)
        {
            //Is a slot under drop point
            if (dropSlot.currentItem != null)
            {
                Item draggedItem = GetComponent<Item>();
                Item targetItem = dropSlot.currentItem.GetComponent<Item>();

                if (draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                }
                else
                {
                    //Slot has an item - swap items
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    transform.SetParent(dropSlot.transform);
                    dropSlot.currentItem = gameObject;
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center
                }
            }
            else
            {

                originalSlot.currentItem = null;
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center
            }
        }
        else
        {
            //No slot under drop point
            //If where we're dropping is not within the inventory
            if (!IsWithinInventory(eventData.position))
            {
                //Drop our item
                DropItem(originalSlot);
            }
            else
            {
                //Snap back to og slot
                transform.SetParent(originalParent);
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center
            }
        }
    }

    bool IsWithinInventory(Vector2 mousePosition)
    {
        if (inventoryController == null || inventoryController.inventoryPanel == null)
            return false;

        RectTransform inventoryRect = inventoryController.inventoryPanel.GetComponent<RectTransform>();
        if (inventoryRect == null)
            return false;

        return RectTransformUtility.RectangleContainsScreenPoint(inventoryRect, mousePosition, null);
    }

    void DropItem(Slot originalSlot)
    {
        Item item = GetComponent<Item>();
        if (item == null) return;

        int quantity = item.quantity;

        if (quantity > 1)
        {
            item.RemoveFromStack(1);

            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            quantity = 1;
        }
        else
        {
            originalSlot.currentItem = null;
        }

        // find player
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.Log("player tag missing");
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            if (originalSlot.currentItem == null)
            {
                originalSlot.currentItem = gameObject;
            }
            return;
        }

        // random drop pos
        Vector2 dropOffset = Random.insideUnitCircle.normalized * Random.Range(minDropDistance, maxDropDistance);
        Vector2 dropPosition = (Vector2)playerTransform.position + dropOffset;

        // instantiate dropped item from the current item like your original code
        GameObject droppedObject = Instantiate(gameObject, dropPosition, Quaternion.identity);
        droppedObject.transform.SetParent(null);

        Item droppedItem = droppedObject.GetComponent<Item>();
        if (droppedItem != null)
        {
            droppedItem.quantity = 1;
            droppedItem.UpdateQuantityDisplay();
        }

        // make sure it is a world pickup
        droppedObject.tag = "Item";

        Collider2D col = droppedObject.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }

        SpriteRenderer sr = droppedObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
        }

        CanvasGroup droppedCanvasGroup = droppedObject.GetComponent<CanvasGroup>();
        if (droppedCanvasGroup != null)
        {
            droppedCanvasGroup.blocksRaycasts = false;
            droppedCanvasGroup.alpha = 1f;
        }

        ItemDragHandler droppedDragHandler = droppedObject.GetComponent<ItemDragHandler>();
        if (droppedDragHandler != null)
        {
            droppedDragHandler.enabled = false;
        }


        BounceEffect bounce = droppedObject.GetComponent<BounceEffect>();
        if (bounce != null)
        {
            bounce.StartBounce();
        }

        // destroy ui item if stack is finished
        if (quantity <= 1 && originalSlot.currentItem == null)
        {
            Destroy(gameObject);
        }

        InventoryController.Instance.RebuildItemCounts();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            SplitStack();
        }
    }

    private void SplitStack()
    {
        Item item = GetComponent<Item>();
        if (item == null || item.quantity <= 1) return;

        int splitAmount = item.quantity / 2;
        if (splitAmount <= 0) return;

        item.RemoveFromStack(splitAmount); 

        GameObject newItem = item.CloneItem(splitAmount);

        if(inventoryController == null || newItem == null) return;

        foreach (Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                slot.currentItem = newItem;
                newItem.transform.SetParent(slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }

        // no empty slot - return to stack
        item.AddToStack(splitAmount);
        Destroy(newItem);
    }
}
