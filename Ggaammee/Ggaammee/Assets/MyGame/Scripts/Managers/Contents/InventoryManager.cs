using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager
{
    public Dictionary<int, Item> Items { get; private set; } = new Dictionary<int, Item>();
    public Action<int, Item> ChangeItemAction;

    public int SlotLen
    {
        get { return Util.StaticValues.InventorySize; }
        private set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            SlotLen = value;
        }
    }


    public int Add(ItemData itemData, int amount = 1)
    {
        int index;

        // 1. 수량이 있는 아이템
        if (itemData is StackableItemData ciData)
        {
            bool findNextCountable = true;
            index = -1;

            while (amount > 0)
            {
                // 1-1. 이미 해당 아이템이 인벤토리 내에 존재하고, 개수 여유 있는지 검사
                if (findNextCountable)
                {
                    index = FindCountableItemSlotIndex(ciData, index + 1);

                    // 개수 여유있는 기존재 슬롯이 더이상 없다고 판단될 경우, 빈 슬롯부터 탐색 시작
                    if (index == -1)
                    {
                        findNextCountable = false;
                    }
                    // 기존재 슬롯을 찾은 경우, 양 증가시키고 초과량 존재 시 amount에 초기화
                    else
                    {
                        StackableItem ci = Items[index] as StackableItem;
                        amount = ci.AddAmountAndGetExcess(amount);

                        UpdateSlot(index, Items[index]);
                    }
                }
                // 1-2. 빈 슬롯 탐색
                else
                {
                    index = FindEmptySlotIndex(index + 1);

                    // 빈 슬롯조차 없는 경우 종료
                    if (index == -1)
                    {
                        break;
                    }
                    // 빈 슬롯 발견 시, 슬롯에 아이템 추가 및 잉여량 계산
                    else
                    {
                        // 새로운 아이템 생성
                        StackableItem ci = ciData.CreateItem() as StackableItem;
                        ci.SetAmount(amount);

                        // 슬롯에 추가
                        Items[index] = ci;

                        // 남은 개수 계산
                        amount = (amount > ciData.maxStack) ? (amount - ciData.maxStack) : 0;

                        UpdateSlot(index, Items[index]);
                    }
                }
            }
        }
        // 2. 수량이 없는 아이템
        else
        {
            // 2-1. 1개만 넣는 경우, 간단히 수행
            if (amount == 1)
            {
                index = FindEmptySlotIndex();
                if (index != -1)
                {
                    // 아이템을 생성하여 슬롯에 추가
                    Items[index] = itemData.CreateItem();
                    amount = 0;

                    UpdateSlot(index, Items[index]);
                }
            }

            // 2-2. 2개 이상의 수량 없는 아이템을 동시에 추가하는 경우
            index = -1;
            for (; amount > 0; amount--)
            {
                // 아이템 넣은 인덱스의 다음 인덱스부터 슬롯 탐색
                index = FindEmptySlotIndex(index + 1);

                // 다 넣지 못한 경우 루프 종료
                if (index == -1)
                {
                    break;
                }

                // 아이템을 생성하여 슬롯에 추가
                Items[index] = itemData.CreateItem();

                UpdateSlot(index, Items[index]);
            }
        }

        return amount;
    }


    private void UpdateSlot(int index, Item item)
    {
        ChangeItemAction.Invoke(index, item);
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

    public Item Get(int Id)
    {
        Items.TryGetValue(Id, out var item);
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