using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    // 아이템 딕셔너리, 키는 슬롯 인덱스
    public Dictionary<int, Item> Items { get; private set; } = new Dictionary<int, Item>();

    public List<Item> Currency = new List<Item>();

    // 아이템 변경 시 호출되는 이벤트
    public event Action<int, Item> ChangeItemAction;

    // 아이템 변경 시 호출되는 이벤트
    public event Action ChangeCurrencyAction;

    public InventoryManager()
    {
        var index = 0;
        foreach (var item in Managers.Instance.StartItemData)
        {
            if (item.data is CurrencyItemData currencyItemData)
            {
                CurrencyItem newItem = currencyItemData.CreateItem() as CurrencyItem;
                newItem.SetAmount(item.count);

                Currency.Add(newItem);
            }
            else if (item.data is UsableItemData usableItemData)
            {
                index = FindEmptySlotIndex();
                UsableItem newItem = usableItemData.CreateItem() as UsableItem;
                var count = item.count;
                newItem.SetAmount(count);
                Items[index] = newItem;

                count = (count > usableItemData.maxStack) ? (count - usableItemData.maxStack) : 0;
                UpdateSlot(index, Items[index]);
            }

            else if (item.data is EuqipItemData data)
            {
                index = FindEmptySlotIndex();
                if (index != -1)
                {
                    Items[index] = data.CreateItem();
                    UpdateSlot(index, Items[index]);
                }
            }
        }
        //Currency.Add();
    }

    // 슬롯 개수
    public int SlotLen
    {
        get { return Util.StaticValues.InventorySize; }
        private set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            SlotLen = value;
        }
    }

    // 유효한 인덱스인지 확인하는 함수
    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < SlotLen;
    }

    // 아이템을 추가하는 함수
    public int Add(ItemData itemData, int amount = 1)
    {
        int index;

        // 스택 가능한 아이템일 경우
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

                    if (index == -1) break;

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
            // 스택 불가능한 아이템의 경우
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

                if (index == -1) break;

                Items[index] = itemData.CreateItem();
                UpdateSlot(index, Items[index]);
            }
        }

        return amount;
    }

    // 두 슬롯을 교환하는 함수
    public void Swap(int fromIndex, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex)) return;

        // 슬롯 인덱스의 아이템 가져오기
        Items.TryGetValue(fromIndex, out var fromItem);
        Items.TryGetValue(toIndex, out var toItem);

        if (fromItem != null && toItem != null && fromItem.Data == toItem.Data && fromItem is StackableItem fi && toItem is StackableItem ti)
        {
            // 스택 가능한 아이템일 경우
            int maxAmount = ti.MaxCount;
            int sum = fi.Count + ti.Count;

            if (sum <= maxAmount)
            {
                fi.SetAmount(0);
                ti.SetAmount(sum);
                Items.Remove(fromIndex);
                Items[toIndex] = ti;
                UpdateSlot(fromIndex, null);
                UpdateSlot(toIndex, Items[toIndex]);
            }
            else
            {
                fi.SetAmount(sum - maxAmount);
                ti.SetAmount(maxAmount);
                Items[fromIndex] = fi;
                Items[toIndex] = ti;
                UpdateSlot(fromIndex, Items[fromIndex]);
                UpdateSlot(toIndex, Items[toIndex]);
            }
        }
        else
        {
            // 일반 아이템 교환
            Items[fromIndex] = toItem;
            Items[toIndex] = fromItem;
            UpdateSlot(fromIndex, Items[fromIndex]);
            UpdateSlot(toIndex, Items[toIndex]);
        }
    }

    // 슬롯 업데이트 이벤트 호출
    private void UpdateSlot(int index, Item item)
    {
        ChangeItemAction?.Invoke(index, item);
    }

    // 비어 있는 슬롯 인덱스를 찾는 함수
    private int FindEmptySlotIndex(int startIndex = 0)
    {
        for (int i = startIndex; i < SlotLen; i++)
        {
            if (!Items.ContainsKey(i))
                return i;
        }

        return -1;
    }

    // 스택 가능한 아이템의 슬롯을 찾는 함수
    private int FindCountableItemSlotIndex(StackableItemData target, int startIndex = 0)
    {
        for (int i = startIndex; i < SlotLen; i++)
        {
            if (!Items.TryGetValue(i, out var current)) continue;

            if (current.Data == target && current is StackableItem ci)
            {
                if (!ci.IsMax)
                    return i;
            }
        }

        return -1;
    }

    // 아이템 수량을 분리하는 함수
    public void SeparateAmount(int indexA, int indexB, int amount)
    {
        if (!IsValidIndex(indexA) || !IsValidIndex(indexB)) return;

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

    // 현재 아이템 수량을 반환하는 함수
    public int GetCurrentAmount(int index)
    {
        if (!IsValidIndex(index)) return -1;
        if (Items[index] == null) return 0;

        StackableItem ci = Items[index] as StackableItem;
        if (ci == null)
            return 1;

        return ci.Count;
    }

    // 아이템 제거 함수
    public void Remove(int index)
    {
        if (!IsValidIndex(index)) return;

        Items[index] = null;
        UpdateSlot(index, null);
    }

    // 아이템 이름을 반환하는 함수
    public string GetItemName(int index)
    {
        if (!IsValidIndex(index)) return "";
        if (Items[index] == null) return "";

        return Items[index].Data.name;
    }

    // 특정 인덱스의 아이템을 반환하는 함수
    public Item Get(int index)
    {
        Items.TryGetValue(index, out var item);
        return item;
    }

    // 조건에 맞는 첫 번째 아이템을 찾는 함수
    public Item Find(Func<Item, bool> condition)
    {
        foreach (Item item in Items.Values)
        {
            if (condition.Invoke(item))
                return item;
        }

        return null;
    }

    // 아이템을 모두 제거하는 함수
    public void Clear()
    {
        Items.Clear();
    }
}