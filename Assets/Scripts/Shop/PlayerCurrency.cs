using UnityEngine;
using System;

/// <summary>
/// Manages player's Scrap currency
/// </summary>
public class PlayerCurrency : MonoBehaviour
{
    public static PlayerCurrency Instance { get; private set; }

    [SerializeField] private int currentScrap = 100; // Starting scrap amount

    public event Action<int> OnScrapChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetScrap()
    {
        return currentScrap;
    }

    public bool HasEnoughScrap(int amount)
    {
        return currentScrap >= amount;
    }

    public bool SpendScrap(int amount)
    {
        if (HasEnoughScrap(amount))
        {
            currentScrap -= amount;
            OnScrapChanged?.Invoke(currentScrap);
            return true;
        }
        return false;
    }

    public void AddScrap(int amount)
    {
        currentScrap += amount;
        OnScrapChanged?.Invoke(currentScrap);
    }
}
