using System;
using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    public static CurrencyController Instance;

    [SerializeField] private int startingGold = 5;
    private int playerGold = 5;
    public event Action<int> onGoldChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
            playerGold = startingGold;
        }
    }

    public int GetGold() => playerGold;

    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            onGoldChanged?.Invoke(playerGold);
            return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
        onGoldChanged?.Invoke(playerGold);
    }

    public void SetGold(int amount)
    {
        playerGold = amount;
        onGoldChanged?.Invoke(playerGold);
    }
}
