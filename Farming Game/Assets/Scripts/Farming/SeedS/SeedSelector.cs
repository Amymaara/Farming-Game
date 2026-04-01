using UnityEngine;

public class SeedSelector : MonoBehaviour
{
    public static SeedSelector Instance { get; private set; }

    public SeedData selectedSeed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SelectSeed(SeedData newSeed)
    {
        if (newSeed == null) return;

        selectedSeed = newSeed;
        Debug.Log("Selected seed: " + newSeed.seedName);
    }

    public void ClearSeed()
    {
        selectedSeed = null;
    }


}
