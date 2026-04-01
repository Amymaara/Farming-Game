using UnityEngine;

public class SeedDatabase : MonoBehaviour
{
    public static SeedDatabase Instance { get; private set; }

    public SeedData[] allSeeds;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public SeedData GetSeedByItemID(int itemID)
    {
        foreach (SeedData seed in allSeeds)
        {
            if (seed != null && seed.seedItemID == itemID)
                return seed;
        }

        return null;
    }
}
