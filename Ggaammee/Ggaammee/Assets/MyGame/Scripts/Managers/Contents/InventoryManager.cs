using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager
{
    // 아이템 슬롯 및 통화 정보
    public Dictionary<int, Item> Items { get; private set; } = new Dictionary<int, Item>();
    public List<StackableItem> Currency { get; private set; } = new List<StackableItem>();

    // 슬롯 변경 및 통화 변경 이벤트
    public event Action<int, Item> SlotChanged;
    public event Action CurrencyChanged;

    public int SlotCapacity { get; private set; } = Util.StaticValues.InventorySize;



    //초기 아이템 보여주기용 예시용도
    public void InitializeInventory()
    {
        foreach (var item in Managers.Instance.StartItemData)
        {
            if (item.data is CurrencyItemData currencyData)
                AddCurrencyItem(currencyData, item.count);
            else
                Add(item.data, item.count);
        }
    }

    private bool IsValidIndex(int index) => index >= 0 && index < SlotCapacity;

    // 아이템 추가
    public int Add(ItemData itemData, int amount = 1)
    {
        return itemData switch
        {
            StackableItemData stackableData => AddStackableItem(stackableData, amount),
            _ => AddNonStackableItem(itemData, amount)
        };
    }

    private int AddStackableItem(StackableItemData data, int amount)
    {
        while (amount > 0)
        {
            int stackSlot = FindSlotWithAvailableStack(data);

            if (stackSlot != -1)
            {
                var stackableItem = Items[stackSlot] as StackableItem;
                amount = stackableItem.AddAmountAndGetExcess(amount);
                UpdateSlot(stackSlot, stackableItem);
            }
            else
            {
                int emptySlot = FindEmptySlot();
                if (emptySlot == -1) break;

                var newItem = data.CreateItem() as StackableItem;
                newItem.SetAmount(amount);
                amount = Math.Max(0, amount - data.maxStack);

                Items[emptySlot] = newItem;
                UpdateSlot(emptySlot, newItem);
            }
        }

        return amount;
    }

    private int AddNonStackableItem(ItemData data, int amount)
    {
        for (; amount > 0; amount--)
        {
            int emptySlot = FindEmptySlot();
            if (emptySlot == -1) break;

            Items[emptySlot] = data.CreateItem();
            UpdateSlot(emptySlot, Items[emptySlot]);
        }

        return amount;
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < SlotCapacity; i++)
        {
            if (!Items.ContainsKey(i))
                return i;
        }

        return -1;
    }

    private int FindSlotWithAvailableStack(StackableItemData data)
    {
        foreach (var kvp in Items)
        {
            if (kvp.Value is StackableItem stackable && stackable.Data == data && !stackable.IsMax)
            {
                return kvp.Key;
            }
        }

        return -1;
    }

    private void AddCurrencyItem(CurrencyItemData data, int amount)
    {
        var currency = Currency.FirstOrDefault(c => c.Data == data);

        if (currency != null)
        {
            currency.AddAmountAndGetExcess(amount);
        }
        else
        {
            var newCurrency = data.CreateItem() as CurrencyItem;
            newCurrency.SetAmount(amount);
            Currency.Add(newCurrency);
        }

        UpdateCurrency();
    }

    public void UseCurrency(string currencyId, int amount)
    {
        var currency = Currency.FirstOrDefault(c => c.Data.id == currencyId);
        if (currency != null)
        {
            currency.AddAmountAndGetExcess(-amount);
            UpdateCurrency();
        }
    }

    private void UpdateCurrency()
    {
        CurrencyChanged?.Invoke();
    }

    private void UpdateSlot(int index, Item item)
    {
        SlotChanged?.Invoke(index, item);
    }

    // 아이템 제거
    public void RemoveItem(int index)
    {
        if (!IsValidIndex(index)) return;

        Items.Remove(index);
        UpdateSlot(index, null);
    }

    // 아이템 분리
    public void SeparateAmount(int fromIndex, int toIndex, int amount)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex)) return;

        if (Items[fromIndex] is StackableItem sourceItem && Items[toIndex] == null)
        {
            Items[toIndex] = sourceItem.SeperateAndClone(amount);
            UpdateSlot(fromIndex, sourceItem);
            UpdateSlot(toIndex, Items[toIndex]);
        }
    }

    // 아이템 교환
    public void Swap(int fromIndex, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex)) return;

        Items.TryGetValue(fromIndex, out var fromItem);
        Items.TryGetValue(toIndex, out var toItem);

        if (fromItem is StackableItem sourceStack && toItem is StackableItem targetStack &&
            sourceStack.Data == targetStack.Data)
        {
            int totalAmount = sourceStack.Count + targetStack.Count;
            int maxStack = targetStack.MaxCount;

            targetStack.SetAmount(Math.Min(totalAmount, maxStack));
            sourceStack.SetAmount(Math.Max(0, totalAmount - maxStack));

            if (sourceStack.Count == 0)
                Items.Remove(fromIndex);

            UpdateSlot(fromIndex, Items.TryGetValue(fromIndex, out var updatedFrom) ? updatedFrom : null);
            UpdateSlot(toIndex, targetStack);
        }
        else
        {
            Items[fromIndex] = toItem;
            Items[toIndex] = fromItem;
            UpdateSlot(fromIndex, toItem);
            UpdateSlot(toIndex, fromItem);
        }
    }

    // 통화 비용 확인 및 사용
    public bool CheckCurrencyCost(string currencyId, int amount)
    {
        var currency = Currency.FirstOrDefault(c => c.Data.id == currencyId);
        return currency != null && currency.CheckCount(amount);
    }

    // 유틸리티 메서드
    public Item GetSlotData(int index) => Items.TryGetValue(index, out var item) ? item : null;

    public int GetAvailableSlot(ItemData item)
    {
        if (item is StackableItemData stackable)
        {
            int slot = FindSlotWithAvailableStack(stackable);
            if (slot != -1) return slot;
        }

        return FindEmptySlot();
    }

    public string GetItemName(int index) =>
        IsValidIndex(index) && Items.TryGetValue(index, out var item) ? item.Data.name : string.Empty;
}