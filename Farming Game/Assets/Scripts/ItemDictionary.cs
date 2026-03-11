using System.Collections.Generic;
using UnityEngine;

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
