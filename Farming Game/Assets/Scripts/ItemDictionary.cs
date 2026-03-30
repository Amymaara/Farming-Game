using System.Collections.Generic;
using UnityEngine;

// Reference:
// Game Code Library (2026) Top Down Tutorial Series - Unity 2D
// YouTube. Available at:
// https://www.youtube.com/playlist?list=PLaaFfzxy_80HtVvBnpK_IjSC8_Y9AOhuP
// (Accessed: 11 March 2026)
// Note:
// Code was adapted and modified by Ammaarah Cassim for project.
// Debugging assistance provided using ChatGPT (OpenAI).

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;

    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        foreach (Item item in itemPrefabs)
        {
            if (item == null) continue;

            if (itemDictionary.ContainsKey(item.ID))
            {
                Debug.LogWarning("Duplicate item ID found: " + item.ID);
                continue;
            }

            itemDictionary.Add(item.ID, item.gameObject);
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);

        if (prefab == null)
        {
            Debug.LogWarning(itemID + " not found in dictionary");
        }

        return prefab;
    } 
}
