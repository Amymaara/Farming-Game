using System.Collections.Generic;
using UnityEngine;

public class SeedBagUI : MonoBehaviour
{
    public GameObject panel;
    public Transform buttonParent;
    public GameObject seedButtonPrefab;
    public SeedData[] allSeeds;

    public void Open()
    {
        if (panel == null) return;
        panel.SetActive(true);
        RefreshSeedList();
    }

    public void Close()
    {
        if (panel == null) return;
        panel.SetActive(false);
    }

    public void Toggle()
    {
        if (panel == null) return;

        if (panel.activeSelf) Close();
        else Open();
    }

    public void RefreshSeedList()
    {
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        if (InventoryController.Instance == null) return;

        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach (SeedData seed in allSeeds)
        {
            if (seed == null) continue;

            int amount = itemCounts.GetValueOrDefault(seed.seedItemID);
            if (amount <= 0) continue;

            GameObject buttonObj = Instantiate(seedButtonPrefab, buttonParent);
            SeedButtonUI buttonUI = buttonObj.GetComponent<SeedButtonUI>();

            if (buttonUI != null)
            {
                buttonUI.Setup(seed, amount, this);
            }
        }
    }

    public void SelectSeed(SeedData seed)
    {
        if (SeedSelector.Instance != null)
        {
            SeedSelector.Instance.SelectSeed(seed);
        }

        Close();
    }
}
