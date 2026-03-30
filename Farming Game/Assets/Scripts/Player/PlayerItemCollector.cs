using UnityEngine;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).
public class PlayerItemCollector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item")) return;

        Item item = collision.GetComponent<Item>();
        if (item == null) return;

        int amountCollected = item.quantity > 0 ? item.quantity : 1;
        int itemID = item.ID;

        bool pickedUp = item.ShowPopUp();

        if (pickedUp)
        {
            if (QuestController.Instance != null)
            {
                QuestController.Instance.RegisterCollectedItem(itemID, amountCollected);
            }
        }
    }


}
