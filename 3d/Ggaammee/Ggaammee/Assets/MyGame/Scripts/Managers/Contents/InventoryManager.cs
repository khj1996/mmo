using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager
{
    public Dictionary<int, Item> Items { get; private set; } = new Dictionary<int, Item>();
    public List<StackableItem> Currency { get; private set; } = new List<StackableItem>();
    public Dictionary<EquipType, EquipItem> EquipedItems { get; private set; } = new Dictionary<EquipType, EquipItem>();

    public event Action<int, Item> SlotChanged;
    public event Action CurrencyChanged;
    public event Action<EquipType> OnEquipChanged;

    public int SlotCapacity { get; private set; } = StaticValues.InventorySize;


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

    public int Add(ItemData itemData, int amount = 1)
    {
        EventManager.TriggerItemCollected(itemData.id, amount);
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

    public void EquipItem(ItemSlotUI slotData)
    {
        if (slotData.Item?.Data is EquipItemData data)
        {
            UnequipItem(data.type);

            EquipNewItem(slotData, data.type);
        }
    }

    public void UnequipItem(EquipType type)
    {
        if (EquipedItems.TryGetValue(type, out var currentEquip))
        {
            var slotIndex = FindEmptySlot();
            if (slotIndex == -1)
            {
                return;
            }

            Items.Add(slotIndex, currentEquip);
            UpdateSlot(slotIndex, currentEquip);
            EquipedItems.Remove(type);
            NotifyEquipChanged(type);
        }
    }

    private void EquipNewItem(ItemSlotUI slotData, EquipType type)
    {
        var selectSlotItem = slotData.Item;
        Items.Remove(slotData.Index);
        UpdateSlot(slotData.Index, null);
        EquipedItems.Add(type, selectSlotItem as EquipItem);
        NotifyEquipChanged(type);
    }


    private void NotifyEquipChanged(EquipType type)
    {
        OnEquipChanged?.Invoke(type);
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
            if (kvp.Value is StackableItem stackable && stackable.StackableData == data && !stackable.IsMax)
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

    public void RemoveItem(int index)
    {
        if (!IsValidIndex(index)) return;

        Items.Remove(index);
        UpdateSlot(index, null);
    }

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
            if (toItem == null)
            {
                Items.Remove(fromIndex);
            }
            else
            {
                Items[fromIndex] = toItem;
            }

            Items[toIndex] = fromItem;

            UpdateSlot(fromIndex, Items.GetValueOrDefault(fromIndex));
            UpdateSlot(toIndex, fromItem);
        }
    }

    public int GetItemAmount(string itemId)
    {
        return Items.Where(x => x.Value.Data.id == itemId && x.Value is StackableItem).Sum(x => ((StackableItem)x.Value).Count);
    }

    public bool CheckCurrencyCost(string currencyId, int amount)
    {
        var currency = Currency.FirstOrDefault(c => c.Data.id == currencyId);
        return currency != null && currency.CheckCount(amount);
    }

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
        IsValidIndex(index) && Items.TryGetValue(index, out var item) ? item.Data.itemName : string.Empty;

    public EquipItem GetEquippedItem(EquipType type)
    {
        return EquipedItems.GetValueOrDefault(type);
    }
}