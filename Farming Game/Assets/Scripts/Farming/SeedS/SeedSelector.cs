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

    public void SelectSeed(SeedData seed)
    {
        selectedSeed = seed;
        Debug.Log("Selected seed set to: " + (seed != null ? seed.seedName : "None"));
    }

    public void ClearSeed()
    {
        selectedSeed = null;
    }


}
