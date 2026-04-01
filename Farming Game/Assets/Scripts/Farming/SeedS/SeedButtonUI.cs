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
        seedData = seed;
        seedBagUI = ui;

        if (icon != null) icon.sprite = seed.seedIcon;
        if (nameText != null) nameText.text = seed.seedName;
        if (amountText != null) amountText.text = "x" + amount;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClicked);
        }
    }

    void OnClicked()
    {
        if (seedBagUI != null && seedData != null)
        {
            seedBagUI.SelectSeed(seedData);
        }
    }
}
