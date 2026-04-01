using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeedButtonUI : MonoBehaviour
{

    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;
    public Button button;

    private SeedData seedData;
    private SeedBagUI seedBagUI;

    public void Setup(SeedData seed, int amount, SeedBagUI ui)
    {
        Debug.Log("SeedButtonUI.Setup called for: " + seed.seedName);

        seedData = seed;
        seedBagUI = ui;

        if (icon != null)
            icon.sprite = seed.seedIcon;

        if (nameText != null)
            nameText.text = seed.seedName;

        if (amountText != null)
            amountText.text = "x" + amount;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
            Debug.Log("Button listener added for: " + seed.seedName);
        }
        else
        {
            Debug.LogError("Button reference missing on SeedButtonUI");
        }
    }

    private void OnClicked()
    {
        Debug.Log("Seed button clicked: " + (seedData != null ? seedData.seedName : "NULL"));

        if (seedBagUI != null && seedData != null)
        {
            seedBagUI.SelectSeed(seedData);
        }
        else
        {
            Debug.LogError("seedBagUI or seedData is null in OnClicked");
        }
    }
}
