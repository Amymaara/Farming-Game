using UnityEngine;

public class TutorialProgress : MonoBehaviour
{
    public static TutorialProgress Instance { get; private set; }

    public bool inventoryShown;
    public bool firstTileHoed;
    public bool seedSelectionShown;
    public bool firstCropPlanted;
    public bool firstCropWatered;
    public bool firstCropHarvested;
    public bool showedToolScroll;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
}
