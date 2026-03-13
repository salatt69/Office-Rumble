using System;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] int money;

    public Action OnMoneyChanged;

    public int Money => money;

    public void AddMoney(int amount)
    {
        money += Mathf.Max(0, amount);
        OnMoneyChanged?.Invoke();
    }

    public bool CanAfford(int amount)
    {
        return money >= amount;
    }

    public bool TrySpend(int amount)
    {
        if (amount < 0) return false;
        if (money < amount) return false;

        money -= amount;
        OnMoneyChanged?.Invoke();

        return true;
    }
}