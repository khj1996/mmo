using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    public Dictionary<int, Item> Items { get; private set; } = new Dictionary<int, Item>();
    public event Action<int, Item> ChangeItemAction;

    public int SlotLen
    {
        get { return Util.StaticValues.InventorySize; }
        private set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            SlotLen = value;
        }
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < SlotLen;
    }


    public int Add(ItemData itemData, int amount = 1)
    {
        int index;

        if (itemData is StackableItemData ciData)
        {
            bool findNextCountable = true;
            index = -1;

            while (amount > 0)
            {
                if (findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index + 1);

                    if (index == -1)
                    {
                        findNextCountable = false;
                    }
                    else
                    {
                        StackableItem ci = Items[index] as StackableItem;
                        amount = ci.AddAmountAndGetExcess(amount);

                        UpdateSlot(index, Items[index]);
                    }
                }
                else
                {
                    index = FindEmptySlotIndex(index + 1);

                    if (index == -1)
                    {
                        break;
                    }

                    StackableItem ci = ciData.CreateItem() as StackableItem;
                    ci.SetAmount(amount);

                    Items[index] = ci;

                    amount = (amount > ciData.maxStack) ? (amount - ciData.maxStack) : 0;

                    UpdateSlot(index, Items[index]);
                }
            }
        }
        else
        {
            if (amount == 1)
            {
                index = FindEmptySlotIndex();
                if (index != -1)
                {
                    Items[index] = itemData.CreateItem();
                    amount = 0;

                    UpdateSlot(index, Items[index]);
                }
            }

            index = -1;
            for (; amount > 0; amount--)
            {
                index = FindEmptySlotIndex(index + 1);

                if (index == -1)
                {
                    break;
                }

                Items[index] = itemData.CreateItem();

                UpdateSlot(index, Items[index]);
            }
        }

        return amount;
    }


    public void Swap(int indexA, int indexB)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Items.TryGetValue(indexA, out var itemA);
        Items.TryGetValue(indexB, out var itemB);

        if (itemA != null && itemB != null && itemA.Data == itemB.Data && itemA is StackableItem ciA && itemB is StackableItem ciB)
        {
            int maxAmount = ciB.MaxCount;
            int sum = ciA.Count + ciB.Count;

            if (sum <= maxAmount)
            {
                ciA.SetAmount(0);
                ciB.SetAmount(sum);
            }
            else
            {
                ciA.SetAmount(sum - maxAmount);
                ciB.SetAmount(maxAmount);
            }
        }
        else
        {
            Items[indexA] = itemB;
            Items[indexB] = itemA;
        }

        UpdateSlot(indexA, Items[indexA]);
        UpdateSlot(indexB, Items[indexB]);
    }

    private void UpdateSlot(int index, Item item)
    {
        ChangeItemAction?.Invoke(index, item);
    }

    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < SlotLen; i++)
            if (!Items.TryGetValue(i, out _))
                return i;
        return -1;
    }

    private int FindCountableItemSlotIndex(StackableItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < SlotLen; i++)
        {
            if (!Items.TryGetValue(i, out var current))
                continue;

            if (current.Data == target && current is StackableItem ci)
            {
                if (!ci.IsMax)
                    return i;
            }
        }

        return -1;
    }

    public void SeparateAmount(int indexA, int indexB, int amount)
    {
        if (!IsValidIndex(indexA)) return;
        if (!IsValidIndex(indexB)) return;

        Item _itemA = Items[indexA];
        Item _itemB = Items[indexB];

        StackableItem _ciA = _itemA as StackableItem;

        if (_ciA != null && _itemB == null)
        {
            Items[indexB] = _ciA.SeperateAndClone(amount);
            UpdateSlot(indexA, _itemA);
            UpdateSlot(indexB, _itemB);
        }
    }

    public int GetCurrentAmount(int index)
    {
        if (!IsValidIndex(index)) return -1;
        if (Items[index] == null) return 0;

        StackableItem ci = Items[index] as StackableItem;
        if (ci == null)
            return 1;

        return ci.Count;
    }

    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        Items[index] = null;
        UpdateSlot(index, null);
    }

    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (Items[index] == null) return "";

        return Items[index].Data.name;
    }

    public Item Get(int index)
    {
        Items.TryGetValue(index, out var item);
        return item;
    }

    public Item Find(Func<Item, bool> condition)
    {
        foreach (Item item in Items.Values)
        {
            if (condition.Invoke(item))
                return item;
        }

        return null;
    }

    public void Clear()
    {
        Items.Clear();
    }
}