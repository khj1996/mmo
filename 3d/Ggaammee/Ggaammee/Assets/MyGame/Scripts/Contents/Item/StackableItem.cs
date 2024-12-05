using System;
using UnityEngine;

public abstract class StackableItem : Item
{
    public StackableItemData StackableData { get; private set; }

    public int Count { get; protected set; }

    public int MaxCount => StackableData.maxStack;

    public bool IsMax => Count >= StackableData.maxStack;

    public bool IsEmpty => Count <= 0;

    public event Action OnChangeCount;


    public StackableItem(StackableItemData data, int amount = 1) : base(data)
    {
        StackableData = data;
        SetAmount(amount);
    }

    public bool CheckCount(int amount)
    {
        return Count >= amount;
    }

    public void SetAmount(int amount)
    {
        Count = Mathf.Clamp(amount, 0, MaxCount);
        InvokeOnChangeCount();
    }

    public int AddAmountAndGetExcess(int amount)
    {
        int nextAmount = Count + amount;
        SetAmount(nextAmount);

        return (nextAmount > MaxCount) ? (nextAmount - MaxCount) : 0;
    }

    public StackableItem SeperateAndClone(int amount)
    {
        if (Count <= 1) return null;

        if (amount > Count - 1)
            amount = Count - 1;

        Count -= amount;
        return Clone(amount);
    }

    protected abstract StackableItem Clone(int amount);

    protected virtual void InvokeOnChangeCount()
    {
        OnChangeCount?.Invoke();
    }
}